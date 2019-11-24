using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    // Con Fig Vars
    [SerializeField] bool waveOffset = false;
    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 2f;
    [SerializeField] float waveOffsetDivider = 1f;
    [SerializeField] float waveOffsetAddition = 0f;

    // State Vars
    Vector3 startingPos;
    float movementFactor;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        CalcMovementFactor();
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }

    private void CalcMovementFactor()
    {
        if (period == 0) return;
        float cycles = Time.time / period;

        const float tau = Mathf.PI * 2f;
        float rawSinWave = Mathf.Sin(cycles * tau);

        movementFactor = ManageOffset(rawSinWave);
    }

    private float ManageOffset(float rawSinWave)
    {
        if (!waveOffset) return rawSinWave;
        if (waveOffsetDivider == 0) waveOffsetDivider = 1;
        return rawSinWave / waveOffsetDivider + waveOffsetAddition;

    }
}
