using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SpaceInvadersText : MonoBehaviour
{
	private struct Bullet
	{
		public int x;

		public int y;

		public int dy;

		public bool fromEnemy;
	}

	[Header("Display")]
	public Text displayText;

	public int cols = 40;

	public int rows = 24;

	[Header("Characters")]
	public char emptyChar = ' ';

	public char playerChar = '^';

	public char enemyChar = 'W';

	public char playerBulletChar = '|';

	public char enemyBulletChar = '!';

	public char wallChar = '▉';

	[Header("Gameplay")]
	public int enemyRows = 3;

	public int enemyCols = 10;

	public float stepInterval = 0.2f;

	public float enemyShootChancePerStep = 0.02f;

	public int playerLives = 3;

	[Header("Controls")]
	public KeyCode leftKey = KeyCode.A;

	public KeyCode rightKey = KeyCode.D;

	public KeyCode shootKey = KeyCode.Space;

	private StringBuilder buffer;

	private char[] lineChars;

	private float stepTimer;

	private int playerX;

	private int playerY;

	private List<Bullet> bullets = new List<Bullet>();

	private bool[,] enemiesAlive;

	private int enemyOffsetX = 4;

	private int enemyStartY = 2;

	private int enemyDir = 1;

	private int enemyBaseX;

	private int score;

	private int livesLeft;

	private bool gameOver;

	private float moveCooldown = 0.12f;

	private float moveTimer;

	private System.Random rng = new System.Random();

	private void Start()
	{
		Debug.Log("SpaceInvadersText started.");
		if (displayText == null)
		{
			displayText = GetComponent<Text>();
		}
		if (displayText == null)
		{
			Debug.LogError("SpaceInvadersText needs a UI.Text component assigned or attached to the same GameObject.");
			base.enabled = false;
			return;
		}
		displayText.supportRichText = false;
		cols = Mathf.Max(10, cols);
		rows = Mathf.Max(6, rows);
		buffer = new StringBuilder(rows * (cols + 1));
		lineChars = new char[cols];
		SetupGame();
		Render();
	}

	private void SetupGame()
	{
		bullets.Clear();
		enemiesAlive = new bool[enemyRows, enemyCols];
		for (int i = 0; i < enemyRows; i++)
		{
			for (int j = 0; j < enemyCols; j++)
			{
				enemiesAlive[i, j] = true;
			}
		}
		enemyBaseX = 0;
		enemyDir = 1;
		playerX = cols / 2;
		playerY = rows - 2;
		score = 0;
		livesLeft = playerLives;
		gameOver = false;
		stepTimer = 0f;
	}

	private void Update()
	{
		if (displayText == null)
		{
			return;
		}
		if (gameOver)
		{
			if (Input.GetKeyDown(KeyCode.R))
			{
				SetupGame();
			}
			return;
		}
		HandleInput();
		stepTimer += Time.deltaTime;
		if (stepTimer >= stepInterval)
		{
			stepTimer -= stepInterval;
			Step();
		}
	}

	private void HandleInput()
	{
		moveTimer -= Time.deltaTime;
		bool flag = false;
		if (moveTimer <= 0f)
		{
			if (Input.GetKey(leftKey))
			{
				playerX = Mathf.Max(1, playerX - 1);
				flag = true;
				moveTimer = moveCooldown;
			}
			else if (Input.GetKey(rightKey))
			{
				playerX = Mathf.Min(cols - 2, playerX + 1);
				flag = true;
				moveTimer = moveCooldown;
			}
		}
		if (Input.GetKeyDown(shootKey))
		{
			bool flag2 = true;
			foreach (Bullet bullet in bullets)
			{
				if (!bullet.fromEnemy && bullet.x == playerX && bullet.y == playerY - 1)
				{
					flag2 = false;
					break;
				}
			}
			if (flag2)
			{
				bullets.Add(new Bullet
				{
					x = playerX,
					y = playerY - 1,
					dy = -1,
					fromEnemy = false
				});
			}
		}
		if (flag)
		{
			Render();
		}
	}

	private void Step()
	{
		for (int num = bullets.Count - 1; num >= 0; num--)
		{
			Bullet value = bullets[num];
			value.y += value.dy;
			if (value.y < 0 || value.y >= rows)
			{
				bullets.RemoveAt(num);
				continue;
			}
			if (!value.fromEnemy)
			{
				int num2 = value.x - enemyOffsetX - enemyBaseX;
				if (num2 >= 0)
				{
					int num3 = num2;
					int num4 = value.y - enemyStartY;
					if (num4 >= 0 && num4 < enemyRows && num3 >= 0 && num3 < enemyCols && enemiesAlive[num4, num3])
					{
						enemiesAlive[num4, num3] = false;
						bullets.RemoveAt(num);
						score += 10;
						continue;
					}
				}
			}
			else if (value.y == playerY && value.x == playerX)
			{
				bullets.RemoveAt(num);
				livesLeft--;
				if (livesLeft <= 0)
				{
					GameOver(playerWon: false);
				}
				continue;
			}
			bullets[num] = value;
		}
		bool flag = false;
		int num5 = int.MaxValue;
		int num6 = int.MinValue;
		bool flag2 = false;
		for (int i = 0; i < enemyRows; i++)
		{
			for (int j = 0; j < enemyCols; j++)
			{
				if (enemiesAlive[i, j])
				{
					flag2 = true;
					int b = enemyOffsetX + enemyBaseX + j;
					num5 = Mathf.Min(num5, b);
					num6 = Mathf.Max(num6, b);
				}
			}
		}
		if (!flag2)
		{
			ResetEnemies();
			Render();
			return;
		}
		if (enemyDir > 0 && num6 + 1 >= cols - 1)
		{
			flag = true;
		}
		if (enemyDir < 0 && num5 - 1 <= 0)
		{
			flag = true;
		}
		if (flag)
		{
			enemyStartY++;
			enemyDir *= -1;
			if (enemyStartY + enemyRows >= playerY)
			{
				GameOver(playerWon: false);
				Render();
				return;
			}
		}
		else
		{
			enemyBaseX += enemyDir;
		}
		for (int k = 0; k < enemyRows; k++)
		{
			for (int l = 0; l < enemyCols; l++)
			{
				if (enemiesAlive[k, l] && rng.NextDouble() < (double)enemyShootChancePerStep)
				{
					int x = enemyOffsetX + enemyBaseX + l;
					int num7 = enemyStartY + k;
					bullets.Add(new Bullet
					{
						x = x,
						y = num7 + 1,
						dy = 1,
						fromEnemy = true
					});
				}
			}
		}
		Render();
	}

	private void ResetEnemies()
	{
		for (int i = 0; i < enemyRows; i++)
		{
			for (int j = 0; j < enemyCols; j++)
			{
				enemiesAlive[i, j] = true;
			}
		}
		enemyStartY = 2;
		enemyBaseX = 0;
		enemyDir = 1;
		stepInterval = Mathf.Max(0.03f, stepInterval * 0.95f);
	}

	private void GameOver(bool playerWon)
	{
		gameOver = true;
	}

	private void Render()
	{
		buffer.Clear();
		string text = $" SCORE: {score}  LIVES: {livesLeft}  (A/D move, SPACE shoot, R restart)";
		AppendLinePadded(text);
		for (int i = 1; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				lineChars[j] = emptyChar;
			}
			lineChars[0] = wallChar;
			lineChars[cols - 1] = wallChar;
			for (int k = 0; k < enemyRows; k++)
			{
				for (int l = 0; l < enemyCols; l++)
				{
					if (enemiesAlive[k, l])
					{
						int num = enemyOffsetX + enemyBaseX + l;
						if (enemyStartY + k == i && num > 0 && num < cols - 1)
						{
							lineChars[num] = enemyChar;
						}
					}
				}
			}
			foreach (Bullet bullet in bullets)
			{
				if (bullet.y == i && bullet.x > 0 && bullet.x < cols - 1)
				{
					lineChars[bullet.x] = (bullet.fromEnemy ? enemyBulletChar : playerBulletChar);
				}
			}
			if (i == playerY && playerX > 0 && playerX < cols - 1)
			{
				lineChars[playerX] = playerChar;
			}
			buffer.Append(lineChars);
			buffer.Append('\n');
		}
		displayText.text = buffer.ToString();
		if (gameOver)
		{
			displayText.text += "\n   GAME OVER - Press R to restart";
		}
	}

	private void AppendLinePadded(string text)
	{
		string text2 = ((text.Length > cols) ? text.Substring(0, cols) : text);
		buffer.Append(text2);
		if (text2.Length < cols)
		{
			buffer.Append(new string(' ', cols - text2.Length));
		}
		buffer.Append('\n');
	}
}
