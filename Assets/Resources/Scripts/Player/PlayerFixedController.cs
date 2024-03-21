public class PlayerFixedController : SuperCharacterController
{

    void Update()
    {
        //Disable SuperCharacterController Update
    }

    public void DoUpdate(float delta)
    {
        base.deltaTime = delta;
        base.SingleUpdate();
    }

}
