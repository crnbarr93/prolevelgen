using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LevelReset : MonoBehaviour , IPointerClickHandler
{

	public void OnPointerClick (PointerEventData data) {

        // reload the scene
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
	}

    private void Update()
    {
    }
}
