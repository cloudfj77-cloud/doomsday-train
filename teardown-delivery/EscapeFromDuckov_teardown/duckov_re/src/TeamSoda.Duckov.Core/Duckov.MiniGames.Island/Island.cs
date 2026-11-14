using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using Eflatun.SceneReference;
using UnityEngine;

namespace Duckov.MiniGames.Island;

public class Island : MiniGameBehaviour
{
	[SerializeField]
	private SceneReference scene;

	[SerializeField]
	private SceneReference curtain;

	[Header("UI")]
	[SerializeField]
	private FadeGroup titleFadeGroup;

	[SerializeField]
	private FadeGroup noGraphicsCardOverlay;

	[SerializeField]
	private PressStartText pressStartText;

	private bool confirmed;

	protected override void Start()
	{
		base.Start();
		CancellationToken cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
		Main(cancellationTokenOnDestroy).Forget();
	}

	private async UniTask Main(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			pressStartText.Reset();
			await DoMainTitle(cancellationToken);
			await WaitForConfirm(cancellationToken);
			pressStartText.NotifyPressed();
			await UniTask.WaitForSeconds(1f);
			if (base.Game.Console != null && !base.Game.Console.HasGraphicsCard)
			{
				DoNoGraphicCards();
				do
				{
					await WaitForConfirm(cancellationToken);
				}
				while (!base.Game.Console.HasGraphicsCard);
				noGraphicsCardOverlay.Hide();
			}
			else
			{
				LoadLevel();
			}
		}
	}

	private void DoNoGraphicCards()
	{
		noGraphicsCardOverlay.Show();
	}

	private void LoadLevel()
	{
		SceneLoader.Instance.LoadScene(scene, curtain).Forget();
	}

	private async UniTask DoMainTitle(CancellationToken cancellationToken)
	{
		await titleFadeGroup.ShowAndReturnTask().AttachExternalCancellation(cancellationToken);
	}

	private async UniTask WaitForConfirm(CancellationToken cancellationToken)
	{
		confirmed = false;
		while (!confirmed)
		{
			await UniTask.Yield(cancellationToken);
		}
	}

	protected override void OnUpdate(float deltaTime)
	{
		if (base.Game.GetButtonDown(MiniGame.Button.Start) || base.Game.GetButtonDown(MiniGame.Button.A))
		{
			confirmed = true;
		}
	}
}
