using UnityEngine;
using Unity.Sentis; 
using Vuforia;
using UnityEngine.Video;

public class AR_AI_IntegratedDetector : MonoBehaviour
{
    [Header("Sentis AI Setup")]
    public ModelAsset modelAsset;
    private Model runtimeModel;
    private Worker worker;

    [Header("Content to Trigger")]
    public ARContentManager contentManager; // Drag your ARContentManager here
    public VideoClip tigerVideo;
    public AudioClip tigerSound;
    public VideoClip dogVideo;
    public AudioClip dogSound;

    private int lastDetectedID = -1;

    void Start() {
        runtimeModel = ModelLoader.Load(modelAsset);
        worker = new Worker(runtimeModel, BackendType.GPUCompute);
        
        // Ensure Vuforia is requesting the right image format
        VuforiaApplication.Instance.OnVuforiaStarted += () => {
            VuforiaBehaviour.Instance.CameraDevice.SetFrameFormat(PixelFormat.RGB888, true);
        };
    }

    void Update() {
        if (!VuforiaApplication.Instance.IsInitialized) return;

        var image = VuforiaBehaviour.Instance.CameraDevice.GetCameraImage(PixelFormat.RGB888);
        if (image == null) return;

        // Process frame for AI
        Texture2D tex = new Texture2D(image.Width, image.Height, TextureFormat.RGB24, false);
        image.CopyToTexture(tex);
        using Tensor<float> input = TextureConverter.ToTensor(tex, 224, 224, 3);

        worker.Schedule(input);
        var output = worker.PeekOutput() as Tensor<float>;
        float[] results = output.DownloadToArray();

        int currentID = GetArgMax(results);
        
        // Only trigger if the animal changes to avoid flickering
        if (currentID != lastDetectedID) {
            CheckAndTriggerContent(currentID);
            lastDetectedID = currentID;
        }
    }

    void CheckAndTriggerContent(int id) {
        Debug.Log("Detecting");
        // IDs based on standard MobileNetV2
        if (id == 292) { // Tiger
            contentManager.OnTargetFound(tigerSound, tigerVideo);
            Debug.Log("Tiger");
        } else if (id >= 151 && id <= 275) { // Any Dog breed
            contentManager.OnTargetFound(dogSound, dogVideo);
            Debug.Log("Dog");

        } else {
            // Optional: Call OnTargetLost if nothing is detected
            contentManager.OnTargetLost();
        }
    }

    int GetArgMax(float[] data) {
        int maxIdx = 0;
        for (int i = 1; i < data.Length; i++)
            if (data[i] > data[maxIdx]) maxIdx = i;
        return maxIdx;
    }

    void OnDestroy() { worker?.Dispose(); }
}