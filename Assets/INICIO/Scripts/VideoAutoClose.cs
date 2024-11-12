using UnityEngine;
using UnityEngine.Video;

public class VideoAutoClose : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    // Drag the Canvas GameObject into this field in the Inspector
    public Canvas videoCanvas;

    void Start()
    {
        // Get the VideoPlayer component
        videoPlayer = GetComponent<VideoPlayer>();

        // Subscribe to the loopPointReached event
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    // This function is called when the video reaches the end
    void OnVideoEnd(VideoPlayer vp)
    {
        // Disable the Canvas GameObject to "close" the video display
        if (videoCanvas != null)
        {
            videoCanvas.gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from the event to avoid potential issues when the GameObject is destroyed
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }
}
