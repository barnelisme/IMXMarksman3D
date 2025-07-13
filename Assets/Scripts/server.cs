using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
public class server : MonoBehaviour
{
	//[SerializeField] GameObject welcomePanel;
	//[SerializeField] Text user;
	//[Space]
	[SerializeField] InputField email;
	[SerializeField] InputField password;

	[SerializeField] Text errorMessages;
	//[SerializeField] GameObject progressCircle;


	[SerializeField] Button loginButton;


	[SerializeField] string url;

	WWWForm form;

	public void OnLoginButtonClicked()
	{
		loginButton.interactable = false;
		//progressCircle.SetActive(true);
		StartCoroutine(Login());
	}

	IEnumerator Login()
	{
		form = new WWWForm();

		form.AddField("email", email.text);
		form.AddField("password", password.text);

		print(email.text);
		print(password.text);
		if(email.text == "admin" && password.text == "GrantAccess@12")//defaut credentials
        {
			SceneManager.LoadScene("MainMenu");
		}

		WWW w = new WWW(url, form);
		yield return w;

		print(w.text);
		if (w.error != null)
		{
			print(w.error);
			errorMessages.text = "404 not found!";
			//Debug.Log("<color=red>" + w.text + "</color>");//error
		}
		else
		{
			print("i'm here");
			if (w.isDone)
			{
				print("i'm here 2");
				if (w.text.Contains("success"))
				{
					print("i'm here 3");
					//open welcom panel
					//	welcomePanel.SetActive(true);
					SceneManager.LoadScene("MainMenu");
					//user.text = username.text;
					//Debug.Log("<color=green>" + w.text + "</color>");//user exist
				}
                else
                {
					print("i'm here 30");
					errorMessages.text = "invalid password or username";
				}
				
			}
		}
		print(w.text);
		loginButton.interactable = true;
		//progressCircle.SetActive(false);

		w.Dispose();
	}
}