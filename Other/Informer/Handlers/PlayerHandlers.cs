using Exiled.Events.EventArgs.Player;
using MEC;

namespace CoreLib.Other.Informer.Handlers
{
    internal static class PlayerHandlers
    {
        internal static void OnVerified(VerifiedEventArgs ev)
        {
            Timing.CallDelayed(15.0f, () => ev.Player.SendConsoleMessage("Сервер разработан командой nu-forward: \nwexels.dev https://github.com/wexelsdev\nPawmi https://github.com/Pawmii\nkloer26(Finor) https://github.com/klo344343\nWater\nalexeyyt4/Футаба https://github.com/alexeyyt4\nSecurity Camera https://github.com/security-camera\nМорковка\nfatal error 404 https://github.com/Fatal-Error-404-developer\nи другими не мение талантливыми людьми", "white"));
        }
    }
}