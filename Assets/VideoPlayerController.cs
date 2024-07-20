using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.IO;

public class VideoPlayerController : MonoBehaviour
{
    public Dropdown dropdown;
    public VideoPlayer videoPlayer;

    private Dictionary<string, string> questionVideoPaths = new Dictionary<string, string>();

    void Start()
    {
        if (dropdown == null)
        {
            Debug.LogError("Dropdown component is not assigned.");
            return;
        }
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer component is not assigned.");
            return;
        }

        videoPlayer.playOnAwake = false;  // Ensure video doesn't play on start

        LoadCSVData();

        // Initialize dropdown with default option
        InitializeDropdownWithDefault();

        // Add listener for when the dropdown value changes
        dropdown.onValueChanged.AddListener(DropdownValueChanged);
    }

    void LoadCSVData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "videos.csv");

        if (File.Exists(filePath))
        {
            var lines = File.ReadAllLines(filePath);

            // Start from 1 to skip header row
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var values = line.Split(',');

                if (values.Length == 2)
                {
                    string question = values[0].Trim();
                    string videoPath = values[1].Trim();

                    questionVideoPaths.Add(question, Path.Combine(Application.streamingAssetsPath, videoPath));
                }
            }

            Debug.Log("CSV data loaded successfully.");
        }
        else
        {
            Debug.LogError("CSV file not found at path: " + filePath);
        }
    }

    void InitializeDropdownWithDefault()
    {
        dropdown.options.Clear();

        // Add the default option
        dropdown.options.Add(new Dropdown.OptionData() { text = " " });

        // Add the options from the CSV
        foreach (var question in questionVideoPaths.Keys)
        {
            dropdown.options.Add(new Dropdown.OptionData() { text = question });
        }

        // Set the default option as the selected one
        dropdown.value = 0;
        dropdown.RefreshShownValue();  // Refresh the dropdown to reflect changes
    }

    void DropdownValueChanged(int change)
    {
        // Ensure the default option is not selected
        if (dropdown.value == 0)
        {
            return;  // Skip processing if the default option is selected
        }

        string selectedQuestion = dropdown.options[change].text;

        if (questionVideoPaths.ContainsKey(selectedQuestion))
        {
            string videoPath = questionVideoPaths[selectedQuestion];
            if (File.Exists(videoPath))
            {
                // Check if the video is currently playing, and if so, stop it.
                if (videoPlayer.isPlaying)
                {
                    videoPlayer.Stop();
                }

                // Set the video URL and prepare to play it.
                videoPlayer.url = videoPath;

                // Play the video
                videoPlayer.Play();
            }
            else
            {
                Debug.LogError("Video file not found at path: " + videoPath);
            }
        }
    }
}
