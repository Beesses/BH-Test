using UnityEngine;

public class InputController : IUpdate
{
    private bool isActive = true;
    PlayerController _playerController;

    public bool IsActive
    {
        get { return isActive; }
        set { isActive = value; }
    }
    public InputController(PlayerController playerController)
    {
        _playerController = playerController;
    }

    public void OnUpdate()
    {
        if (isActive)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            _playerController.Moving(horizontal, vertical);
            if (Input.GetMouseButtonDown(0))
            {
                _playerController.Dashing();
            }
            _playerController.SideStep(Input.GetAxis("Side"));
        }
    }
}
