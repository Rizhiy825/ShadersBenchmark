using System.Collections;
using System.Collections.Generic;
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
    
    public static float currentFps;
    public Label ProfileData { get; set; }

    private static float maxFps;
    private static float averageFps;
    private static int counter;
    
    /*void OnGUI()
    {
        ++counter;
        currentFps = 1.0f / Time.deltaTime;
        
        averageFps += (currentFps - averageFps) / counter;

        if (currentFps > maxFps)
        {
            maxFps = currentFps;
        }

        AverageFpsLabel.text = "Average FPS: " + (int) averageFps;
        MaxFpsLabel.text = "Max FPS: " + (int) maxFps;

        if (counter % 100 == 0)
        {
            CurrentFpsLabel.text = "Current FPS: " + (int) currentFps;
        }
    }*/

    static double GetRecorderFrameAverage(ProfilerRecorder recorder)
    {
        var samplesCount = recorder.Capacity;
        if (samplesCount == 0)
            return 0;

        double r = 0;
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
        sb.AppendLine($"Frame Time: {GetRecorderFrameAverage(mainThreadTimeRecorder) * (1e-6f):F1} ms");
        sb.AppendLine($"GC Memory: {gcMemoryRecorder.LastValue / (1024 * 1024)} MB");
        sb.AppendLine($"System Memory: {systemMemoryRecorder.LastValue / (1024 * 1024)} MB");
        sb.AppendLine($"Draw Calls: {drawCallsCountRecorder.LastValue}");
        sb.AppendLine($"Dynamic Batched Draw Calls Count: {dynamicBatchedDrawCallsCountRecorder.LastValue}");
        sb.AppendLine($"Static Batched Draw Calls Count: {staticBatchedDrawCallsCountRecorder.LastValue}");
        statsText = sb.ToString();
    }
    
    void OnGUI()
    {
        ProfileData.text = statsText;
    }
    
    public void ResetFPS()
    {
        maxFps = 0;
        averageFps = 0;
        counter = 0;
    }
}
