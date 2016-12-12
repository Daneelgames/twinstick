using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DigitPuzzle : MonoBehaviour {
	public List<int> winCombination = new List<int>();
	public List<int> playerCombination = new List<int>();
	public GameObject light;
	public bool complete = false;
	public List<Image> buttonsImage = new List<Image>();
	public List<Text> buttonsText = new List<Text>();

	public InteractiveObject interactiveController;

	void Start()
	{
		SetButtonsInteractive(false);
	}
	public void StartPuzzle(InteractiveObject intr)
	{
		interactiveController = intr;
		SetButtonsInteractive(true);

		if (light)
		{
			light.SetActive(true);
		}
		playerCombination.Clear();
	}

	public void AddNumber(int number)
	{
		playerCombination.Add(number);
		if (playerCombination.Count > winCombination.Count)
		{
			playerCombination.RemoveAt(0);
		}
		CheckResult();
	}

	void CheckResult()
	{
		if (winCombination.Count == playerCombination.Count)
		{
			bool correct = true;

			for (int i = 0; i < winCombination.Count; i ++)
			{
				if (winCombination[i] != playerCombination[i])
				{
						correct = false;
						break;
				}
			}
			if (correct)
			{
				PuzzleComplete();
			}
		}
	}

	void SetButtonsInteractive(bool active)
	{
		if (buttonsImage.Count > 0)
		{
			foreach(Image i in buttonsImage)
			{
				i.raycastTarget = active;
			}
		}
		if (buttonsText.Count > 0)
		{
			foreach(Text t in buttonsText)
			{
				t.raycastTarget = active;
			}
		}
	}

	public void PuzzleOver()
	{
		if (light)
			light.SetActive(false);

		SetButtonsInteractive(false);
	}

	void PuzzleComplete() // player win
	{
		complete = true;
		interactiveController.PuzzleComplete();
	}
}