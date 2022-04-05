using UnityEngine;
using UnityEngine.Video;

using Utility.Attributes;
namespace Utility
{
    public class VideoFitter : MonoBehaviour
    {
        [SerializeField, Require] private VideoPlayer _videoPlayer;
        public float Scale = 1;

        private void OnEnable()
        {
            SetNativeSize();
        }

        public void SetNativeSize()
        {
            var clip = _videoPlayer.clip;
            if (clip == null)
                return;
            var size = new Vector3(clip.width * Scale, clip.height * Scale, 1);
            transform.localScale = size;
        }
    }
}