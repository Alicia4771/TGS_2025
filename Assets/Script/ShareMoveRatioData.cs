using UnityEngine;

public class ShareMoveRatioData : MonoBehaviour
{
    private static double moveRatio;


    public double GetMoveRatio()
    {
        return moveRatio;
    }

    public void SetMoveRatio(double value)
    {
        moveRatio = value;
    }
}
