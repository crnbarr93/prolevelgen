using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class sceneLoader : MonoBehaviour {

    [SerializeField]
    private int scene;
    [SerializeField]
    private Text loadingText;
	private int dotCount = 0;

	private bool loading = false;
    // Updates once per frame
    void Update() {

            // ...set the loadScene boolean to true to prevent loading a new scene more than once...
			string dots = "";
			for(int i = 125; i <= dotCount; i+=125) dots = dots + ".";
			dotCount += 1;
			dotCount = dotCount%500;
            // ...change the instruction text to read "Loading..."
            loadingText.text = "Loading" + dots;
			

            // ...and start a coroutine that will load the desired scene.
            if(!loading) StartCoroutine(LoadNewScene());

		/* 
        // If the new scene has started loading...
        if (loadScene == true) {
            // ...then pulse the transparency of the loading text to let the player know that the computer is still working.
            loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));

        }
		*/

    }


    // The coroutine runs on its own at the same time as Update() and takes an integer indicating which scene to load.
    IEnumerator LoadNewScene() {
		loading = true;
        // This line waits for 3 seconds before executing the next line in the coroutine.
        // This line is only necessary for this demo. The scenes are so simple that they load too fast to read the "Loading..." text.
        //yield return new WaitForSeconds(1);

        // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
        AsyncOperation async = SceneManager.LoadSceneAsync("mainLevel");

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone) {
            yield return null;
        }

    }

}
