using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleAnimator : MonoBehaviour
{
	[Tooltip("Frames to cycle through, in order.")]
	public List<Sprite> sprites = new List<Sprite>();

	[Tooltip("Playback speed in frames per second.")]
	public float framesPerSecond = 8f;

	[Tooltip("Loop back to the first frame after the last one.")]
	public bool loop = true;

	[Tooltip("Play automatically on enable.")]
	public bool playOnEnable = true;

	private Image image;

	private SpriteRenderer spriteRenderer;

	private int currentFrame;

	private float timer;

	private bool playing;

	private void Awake()
	{
		image = GetComponent<Image>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void OnEnable()
	{
		if (playOnEnable)
		{
			Play();
		}
	}

	private void Update()
	{
		if (playing && sprites.Count != 0 && !(framesPerSecond <= 0f))
		{
			timer += Time.deltaTime;
			float num = 1f / framesPerSecond;
			while (timer >= num)
			{
				timer -= num;
				Advance();
			}
		}
	}

	public void Play()
	{
		currentFrame = 0;
		timer = 0f;
		playing = true;
		ApplyFrame();
	}

	public void Stop()
	{
		playing = false;
	}

	private void Advance()
	{
		int num = currentFrame + 1;
		if (num >= sprites.Count)
		{
			if (!loop)
			{
				playing = false;
				return;
			}
			num = 0;
		}
		currentFrame = num;
		ApplyFrame();
	}

	private void ApplyFrame()
	{
		if (sprites.Count != 0)
		{
			Sprite sprite = sprites[currentFrame];
			if (image != null)
			{
				image.sprite = sprite;
			}
			if (spriteRenderer != null)
			{
				spriteRenderer.sprite = sprite;
			}
		}
	}
}
