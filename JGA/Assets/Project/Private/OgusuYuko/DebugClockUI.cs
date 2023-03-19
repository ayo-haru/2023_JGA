using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugClockUI : MonoBehaviour
{
    [SerializeField] private ClockUI ui;
    [SerializeField] private GuestNumUI guestNumUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ui.CountStart();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ui.LossTime();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            guestNumUI.Add();
        }
    }
}
