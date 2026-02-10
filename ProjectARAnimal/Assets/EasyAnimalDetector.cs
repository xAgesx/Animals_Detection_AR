using UnityEngine;
using Unity.Sentis; // Make sure the package is installed
using Vuforia;

public class EasyAnimalDetector : MonoBehaviour
{
    public ModelAsset modelAsset;
    private Model runtimeModel;
    private Worker worker; // Formerly IWorker

    void Start()
    {
        // New way to load and create a worker
        runtimeModel = ModelLoader.Load(modelAsset);
        worker = new Worker(runtimeModel, BackendType.GPUCompute);
    }

    void Update()
    {
        if (!VuforiaBehaviour.Instance.enabled || !VuforiaApplication.Instance.IsInitialized)
        {
            return;
        }

        var image = VuforiaBehaviour.Instance.CameraDevice.GetCameraImage(PixelFormat.RGB888);

        if (image == null)
        {
            VuforiaBehaviour.Instance.CameraDevice.SetFrameFormat(PixelFormat.RGB888, true);
            return;
        }

        Texture2D tex = new Texture2D(image.Width, image.Height, TextureFormat.RGB24, false);
        image.CopyToTexture(tex);

        using Tensor<float> input = TextureConverter.ToTensor(tex, 224, 224, 3);

        worker.Schedule(input);

        Tensor<float> output = worker.PeekOutput() as Tensor<float>;


        float[] results = output.DownloadToArray();

        int bestIdx = GetArgMax(results);
        Debug.Log("Animal ID detected: " + bestIdx);
    }

    int GetArgMax(float[] data)
    {
        int maxIdx = 0;
        for (int i = 1; i < data.Length; i++)
            if (data[i] > data[maxIdx]) maxIdx = i;
        return maxIdx;
    }

    void OnDestroy()
    {
        worker?.Dispose(); // Always clean up AI workers!
    }
}