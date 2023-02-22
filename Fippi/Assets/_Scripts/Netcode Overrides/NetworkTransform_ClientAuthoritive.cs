using Unity.Netcode.Components;
class NetworkTransform_ClientAuthoritive : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}