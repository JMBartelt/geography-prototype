using System;
using System.Text;
using Meta.WitAi.Events;
using UnityEngine;
using Meta.WitAi.Dictation;

public class TranscriptController : MonoBehaviour
{
    [SerializeField] private DictationService witDictation;
    [SerializeField] private int linesBetweenActivations = 2;
    [Multiline]
    [SerializeField] private string activationSeparator = String.Empty;

    [Header("Events")]
    [SerializeField] private WitTranscriptionEvent onTranscriptionUpdated = new
        WitTranscriptionEvent();
    [SerializeField] private WitTranscriptionEvent onInputFinished = new
    WitTranscriptionEvent(); // This is called when the user stops speaking

    private StringBuilder _text; // This is the full transcript
    private string _activeText; // This is the current partial transcript
    private bool _newSection;
    private StringBuilder _separator;

    [SerializeField] private GameObject _micIndicator;

    private void Awake()
    {
        if (!witDictation) witDictation = FindObjectOfType<DictationService>();

        _text = new StringBuilder();
        _separator = new StringBuilder();
        for (int i = 0; i < linesBetweenActivations; i++)
        {
            _separator.AppendLine();
        }

        if (!string.IsNullOrEmpty(activationSeparator))
        {
            _separator.Append(activationSeparator);
        }
    }

    private void OnEnable()
    {
        witDictation.DictationEvents.OnFullTranscription.AddListener(OnFullTranscription);
        witDictation.DictationEvents.OnPartialTranscription.AddListener(OnPartialTranscription);
        witDictation.DictationEvents.OnAborting.AddListener(OnCancelled);

        witDictation.DictationEvents.OnMicStartedListening.AddListener(OnMicStartedListening);
        witDictation.DictationEvents.OnMicStoppedListening.AddListener(OnMicStoppedListening);

        _micIndicator.SetActive(witDictation.MicActive);
    }

    private void OnDisable()
    {
        _activeText = string.Empty;
        witDictation.DictationEvents.OnFullTranscription.RemoveListener(OnFullTranscription);
        witDictation.DictationEvents.OnPartialTranscription.RemoveListener(OnPartialTranscription);
        witDictation.DictationEvents.OnAborting.RemoveListener(OnCancelled);

        witDictation.DictationEvents.OnMicStartedListening.RemoveListener(OnMicStartedListening);
        witDictation.DictationEvents.OnMicStoppedListening.RemoveListener(OnMicStoppedListening);
    }

    private void OnCancelled() // This is called when the user stops speaking
    {
        // Debug.Log("OnCancelled, sending "+ _activeText + " to onInputFinished");
        // onInputFinished.Invoke(_activeText.ToString());
        _activeText = string.Empty;
        OnTranscriptionUpdated();
    }

    private void OnFullTranscription(string text)
    {
        Debug.Log("OnFullTranscription, _activeText = " + _activeText);
        onInputFinished.Invoke(_activeText.ToString());
        _activeText = string.Empty;

        if (_text.Length > 0)
        {
            _text.Append(_separator);
        }

        _text.Append(text);

        OnTranscriptionUpdated();
    }

    private void OnPartialTranscription(string text)
    {
        Debug.Log("OnPartialTranscription, _activeText = " + _activeText);
        _activeText = text;
        OnTranscriptionUpdated();
    }

    public void Clear()
    {
        _text.Clear();
        onTranscriptionUpdated.Invoke(string.Empty);
    }

    private void OnTranscriptionUpdated()
    {
        var transcription = new StringBuilder();
        transcription.Append(_text);
        if (!string.IsNullOrEmpty(_activeText))
        {
            if (transcription.Length > 0)
            {
                transcription.Append(_separator);
            }

            if (!string.IsNullOrEmpty(_activeText))
            {
                transcription.Append(_activeText);
            }
        }

        onTranscriptionUpdated.Invoke(transcription.ToString());
    }

    private void OnMicStartedListening()
    {
        _micIndicator.SetActive(true);
    }

    private void OnMicStoppedListening()
    {
        _micIndicator.SetActive(false);
    }
}

