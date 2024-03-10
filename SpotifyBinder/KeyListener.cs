using Gma.System.MouseKeyHook;
using System.Windows.Forms;

public class InputBindingManager
{
    private IKeyboardMouseEvents _mEvents;

    public string CurrentBinding { get; private set; }

    public event Action<string> OnBindingSet;

    public event Action<string> OnBindingPressed;

    public event Action<string> OnBindingReleased;

    public void Setup()
    {
        _mEvents = Hook.GlobalEvents();
        _mEvents.KeyDown += GlobalHookKeyDown;
        _mEvents.KeyUp += GlobalHookKeyUp;
    }

    private void GlobalHookKeyDown(object sender, KeyEventArgs e)
    {
        OnBindingPressed?.Invoke(e.KeyCode.ToString());
    }

    private void GlobalHookKeyUp(object sender, KeyEventArgs e)
    {
        OnBindingReleased?.Invoke(e.KeyCode.ToString());
    }

    public void StopListening()
    {
        if (_mEvents == null) return;

        _mEvents.KeyDown -= GlobalHookKeyDown;
        _mEvents.KeyUp -= GlobalHookKeyUp;
        _mEvents.Dispose();
        _mEvents = null;
    }
}