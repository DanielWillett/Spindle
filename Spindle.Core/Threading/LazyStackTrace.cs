using DanielWillett.ReflectionTools.Emit;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace Spindle.Threading;

/// <summary>
/// Provides a way to get a <see cref="StackTrace"/>
/// </summary>
public readonly struct LazyStackTrace
{
    private delegate bool GetFileInfo(int skip, bool needFileInfo, out MethodBase method, out int ilOffset, out int nativeOffset, out string file, out int line, out int column);
    private delegate void CreateStackTrace(StackTrace uninitialized, StackFrame[] stackFrames);
    private delegate void CreateStackFrame(StackFrame uninitialized, in SimpleStackFrame stackFrame);

    private static readonly GetFileInfo GetFileInfoMtd;
    private static readonly CreateStackFrame InitializeStackFrameMtd;
    private static readonly CreateStackTrace InitializeStackTraceMtd;

    public readonly SimpleStackFrame[] Frames;

    static LazyStackTrace()
    {
        MethodInfo stackFrameGetInfo = typeof(StackFrame).GetMethod("get_frame_info", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                                       ?? throw new InvalidOperationException("Unable to find 'StackFrame.get_frame_info'. LazyStackTrace is only supported on the Mono runtime.");

        FieldInfo sfFrames = typeof(StackTrace).GetField("frames", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                             ?? throw new InvalidOperationException("Unable to find 'StackTrace.frames'. LazyStackTrace is only supported on the Mono runtime.");

        FieldInfo sfMethodBase = typeof(StackFrame).GetField("methodBase", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                 ?? throw new InvalidOperationException("Unable to find 'StackFrame.methodBase'. LazyStackTrace is only supported on the Mono runtime.");
        FieldInfo ssfMethod = typeof(SimpleStackFrame).GetField(nameof(SimpleStackFrame.Method), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
        FieldInfo sfIlOffset = typeof(StackFrame).GetField("ilOffset", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                               ?? throw new InvalidOperationException("Unable to find 'StackFrame.ilOffset'. LazyStackTrace is only supported on the Mono runtime.");
        FieldInfo ssfIL = typeof(SimpleStackFrame).GetField(nameof(SimpleStackFrame.IL), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
        FieldInfo sfNativeOffset = typeof(StackFrame).GetField("nativeOffset", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                   ?? throw new InvalidOperationException("Unable to find 'StackFrame.nativeOffset'. LazyStackTrace is only supported on the Mono runtime.");
        FieldInfo ssfNative = typeof(SimpleStackFrame).GetField(nameof(SimpleStackFrame.Native), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
        FieldInfo sfFileName = typeof(StackFrame).GetField("fileName", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                               ?? throw new InvalidOperationException("Unable to find 'StackFrame.fileName'. LazyStackTrace is only supported on the Mono runtime.");
        FieldInfo ssfFile = typeof(SimpleStackFrame).GetField(nameof(SimpleStackFrame.File), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
        FieldInfo sfLineNumber = typeof(StackFrame).GetField("lineNumber", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                 ?? throw new InvalidOperationException("Unable to find 'StackFrame.lineNumber'. LazyStackTrace is only supported on the Mono runtime.");
        FieldInfo ssfLine = typeof(SimpleStackFrame).GetField(nameof(SimpleStackFrame.Line), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
        FieldInfo sfColumnNumber = typeof(StackFrame).GetField("columnNumber", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                   ?? throw new InvalidOperationException("Unable to find 'StackFrame.columnNumber'. LazyStackTrace is only supported on the Mono runtime.");
        FieldInfo ssfColumn = typeof(SimpleStackFrame).GetField(nameof(SimpleStackFrame.Column), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;

        GetFileInfoMtd = (GetFileInfo)stackFrameGetInfo.CreateDelegate(typeof(GetFileInfo));


        DynamicMethodInfo<CreateStackTrace> traceInfo = DynamicMethodHelper.Create<CreateStackTrace>("InitializeStackTrace", typeof(LazyStackTrace), initLocals: false);

        traceInfo.DefineParameter(1, ParameterAttributes.None, "uninitialized");
        traceInfo.DefineParameter(2, ParameterAttributes.None, "stackFrames");

        IOpCodeEmitter emit = traceInfo.GetEmitter(debuggable: true);

        emit.LoadArgument(0)
            .LoadArgument(1)
            .SetInstanceFieldValue(sfFrames)

            .Return();

        InitializeStackTraceMtd = traceInfo.CreateDelegate();


        DynamicMethodInfo<CreateStackFrame> frameInfo = DynamicMethodHelper.Create<CreateStackFrame>("InitializeStackFrame", typeof(LazyStackTrace), initLocals: false);

        frameInfo.DefineParameter(1, ParameterAttributes.None, "uninitialized");
        frameInfo.DefineParameter(2, ParameterAttributes.None, "stackFrame");

        emit = frameInfo.GetEmitter(debuggable: true);

        emit.LoadArgument(0)
            .Duplicate(5)

            .LoadArgument(1)
            .LoadInstanceFieldValue(ssfMethod)
            .SetInstanceFieldValue(sfMethodBase)

            .LoadArgument(1)
            .LoadInstanceFieldValue(ssfIL)
            .SetInstanceFieldValue(sfIlOffset)

            .LoadArgument(1)
            .LoadInstanceFieldValue(ssfNative)
            .SetInstanceFieldValue(sfNativeOffset)

            .LoadArgument(1)
            .LoadInstanceFieldValue(ssfFile)
            .SetInstanceFieldValue(sfFileName)

            .LoadArgument(1)
            .LoadInstanceFieldValue(ssfLine)
            .SetInstanceFieldValue(sfLineNumber)

            .LoadArgument(1)
            .LoadInstanceFieldValue(ssfColumn)
            .SetInstanceFieldValue(sfColumnNumber)
            
            .Return();

        InitializeStackFrameMtd = frameInfo.CreateDelegate();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public LazyStackTrace(int skipTraces, bool captureFileInfo)
    {
        skipTraces += 2;
        int amt = GetStackSize(skipTraces);

        SimpleStackFrame[] frames = new SimpleStackFrame[amt];
        for (int i = 0; i < frames.Length; ++i)
        {
            ref SimpleStackFrame fr = ref frames[i];
            GetFileInfoMtd(i + skipTraces, captureFileInfo, out fr.Method, out fr.IL, out fr.Native, out fr.File, out fr.Line, out fr.Column);
        }

        Frames = frames;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private int GetStackSize(int skipTraces)
    {
        int lowBar = skipTraces + 1;
        int incr = 2;
        while (true)
        {
            int check = lowBar + incr;
            incr = Math.Min(incr + 1, 10);

            if (!GetFileInfoMtd(check, false, out _, out _, out _, out _, out _, out _))
            {
                --check;
                while (true)
                {
                    if (!GetFileInfoMtd(check, false, out _, out _, out _, out _, out _, out _))
                    {
                        if (check <= lowBar)
                            return -1;
                        --check;
                    }
                    else
                    {
                        return check + 1 - skipTraces;
                    }
                }
            }

            lowBar = check;
        }
    }

    public StackTrace ToStackTrace()
    {
        StackFrame[] frames = new StackFrame[Frames.Length];
        for (int i = 0; i < Frames.Length; ++i)
        {
            StackFrame frame = (StackFrame)FormatterServices.GetUninitializedObject(typeof(StackFrame));
            InitializeStackFrameMtd(frame, in Frames[i]);
            frames[i] = frame;
        }

        StackTrace trace = (StackTrace)FormatterServices.GetUninitializedObject(typeof(StackTrace));
        InitializeStackTraceMtd(trace, frames);
        return trace;
    }

    public StackFrame GetFrame(int index)
    {
        if (index < 0 || index >= Frames.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        StackFrame frame = (StackFrame)FormatterServices.GetUninitializedObject(typeof(StackFrame));

        InitializeStackFrameMtd(frame, in Frames[index]);
        return frame;
    }

    public struct SimpleStackFrame
    {
        public MethodBase Method;
        public int IL;
        public int Native;
        public string File;
        public int Line;
        public int Column;
    }
}