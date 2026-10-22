using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameSandbox;

public class NPC : InteractableObject
{
    private enum NPCState { Idle, Walk, Talk, Wave }

    // Sprite sheet layout: 4x4 grid of 32x32 frames, one animation per row
    private const int FRAME_SIZE = 32;
    private const int FRAMES_PER_ROW = 4;

    // How close (centre to centre, in world pixels) the player must be for the NPC to notice them
    private const float NOTICE_DISTANCE = 48f;

    private readonly Dictionary<NPCState, Animation> _animations;

    private NPCState _state = NPCState.Idle;
    private bool _playerWasNearby;

    protected override int Size => FRAME_SIZE;
    public override string InteractionPrompt => "Press [E] to Talk";

    public NPC(Texture2D spriteSheet, Vector2 position, string[] firstLines, string[] repeatLines)
        : base(spriteSheet, position, firstLines, repeatLines)
    {
        var animations = Animation.FromGrid(spriteSheet, FRAME_SIZE, FRAMES_PER_ROW,
            ("Idle", 300, true),
            ("Walk", 120, true),
            ("Talk", 150, true),
            ("Wave", 120, false));

        _animations = new Dictionary<NPCState, Animation>
        {
            [NPCState.Idle] = animations["Idle"],
            [NPCState.Walk] = animations["Walk"],   // unused until NPCs move around
            [NPCState.Talk] = animations["Talk"],
            [NPCState.Wave] = animations["Wave"],
        };

        Sprite = new AnimatedSprite(_animations[NPCState.Idle]);
    }

    /// <summary>
    /// Called by the scene every frame so the NPC can react to the player:
    /// waving when they first come close and turning to face them.
    /// </summary>
    public void NoticePlayer(Player player)
    {
        var toPlayer = player.Bounds.Center.ToVector2() - Bounds.Center.ToVector2();
        bool isNearby = toPlayer.Length() <= NOTICE_DISTANCE;

        if (isNearby)
        {
            FacingLeft = toPlayer.X < 0;

            if (!_playerWasNearby && _state == NPCState.Idle)
            {
                SetState(NPCState.Wave);
            }
        }

        _playerWasNearby = isNearby;
    }

    public override void Update(GameTime gameTime)
    {
        // Wave plays once, then the NPC goes back to idling
        if (_state == NPCState.Wave && Sprite.IsFinished)
        {
            SetState(NPCState.Idle);
        }
    }

    public override void OnInteract(DialogueBox dialogue)
    {
        base.OnInteract(dialogue);
        SetState(NPCState.Talk);
    }

    protected override void OnDialogueClosed()
    {
        SetState(NPCState.Idle);
    }

    private void SetState(NPCState state)
    {
        _state = state;
        Sprite.Play(_animations[state]);
    }
}
