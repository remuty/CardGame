using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour
{
    private GameObject gameSystem;

    private GameSystem gameSystemScript;

    public Card card;
    // Start is called before the first frame update
    void Start()
    {
        gameSystem = GameObject.Find("GameSystem");
        gameSystemScript = gameSystem.GetComponent<GameSystem>();

        // カードがクリックされたときのイベントハンドリング
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((data) => { OnPointerDownDelegate((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerDownDelegate(PointerEventData data)
    {
        gameSystemScript.cardClicked(gameObject);
    }
}
