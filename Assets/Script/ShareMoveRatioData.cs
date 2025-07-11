using UnityEngine;

public class ShareMoveRatioData : MonoBehaviour
{
    private double moveRatio;


    public double GetMoveRatio()
    {
        return this.moveRatio;
    }

    public void SetMoveRatio(double moveRatio)
    {
        this.moveRatio = moveRatio;
    }
}
