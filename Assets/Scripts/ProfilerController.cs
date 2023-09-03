using System.Text;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UIElements;

public class ProfilerController : MonoBehaviour
{
    private string statsText;
    private ProfilerRecorder systemMemoryRecorder;
    private ProfilerRecorder gcMemoryRecorder;
    private ProfilerRecorder mainThreadTimeRecorder;
    private ProfilerRecorder drawCallsCountRecorder;
    private ProfilerRecorder dynamicBatchedDrawCallsCountRecorder;
    private ProfilerRecorder staticBatchedDrawCallsCountRecorder;
    
    public Label ProfileData { get; set; }
    public float CurrentFrameTimeInMs { get; private set; }
    
    static float GetRecorderFrameAverage(ProfilerRecorder recorder)
    {
        var samplesCount = recorder.Capacity;
        if (samplesCount == 0)
            return 0;

        float r = 0;
        unsafe
        {
            var samples = stackalloc ProfilerRecorderSample[samplesCount];
            recorder.CopyTo(samples, samplesCount);
            for (var i = 0; i < samplesCount; ++i)
                r += samples[i].Value;
            r /= samplesCount;
        }

        return r;
    }
    
    void OnEnable()
    {
        systemMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
        gcMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
        mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
        drawCallsCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
        dynamicBatchedDrawCallsCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Dynamic Batched Draw Calls Count");
        staticBatchedDrawCallsCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Static Batched Draw Calls Count");
    }

    void OnDisable()
    {
        systemMemoryRecorder.Dispose();
        gcMemoryRecorder.Dispose();
        mainThreadTimeRecorder.Dispose();
        drawCallsCountRecorder.Dispose();
        dynamicBatchedDrawCallsCountRecorder.Dispose();
    }
    
    void Update()
    {
        var sb = new StringBuilder(500);
        CurrentFrameTimeInMs = GetRecorderFrameAverage(mainThreadTimeRecorder) * (1e-6f);
        sb.AppendLine($"Frame Per Second: {1 / (GetRecorderFrameAverage(mainThreadTimeRecorder) * (1e-9f)):F0}");
        sb.AppendLine($"Frame Time: {CurrentFrameTimeInMs:F1} ms");
        sb.AppendLine($"GC Memory: {gcMemoryRecorder.LastValue / (1024 * 1024)} MB");
        sb.AppendLine($"System Memory: {systemMemoryRecorder.LastValue / (1024 * 1024)} MB");
        sb.AppendLine($"Draw Calls: {drawCallsCountRecorder.LastValue}");

        if (UnityEngine.Application.isEditor)
        {
            sb.AppendLine($"Dynamic Batched Draw Calls Count: {dynamicBatchedDrawCallsCountRecorder.LastValue}");
            sb.AppendLine($"Static Batched Draw Calls Count: {staticBatchedDrawCallsCountRecorder.LastValue}");
        }

        statsText = sb.ToString();
    }
    
    void OnGUI()
    {
        ProfileData.text = statsText;
    }
}
