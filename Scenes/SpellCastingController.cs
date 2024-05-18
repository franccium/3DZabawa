using Godot;
using System;
using System.Collections.Generic;

public partial class SpellCastingController : Node
{
    private SpellTree _spellTree;
    private List<StringName> _currSequence;
    private Node3D _spellContainer;
    private VBoxContainer _spellIconsContainer;
    private VBoxContainer _learnedSpellsContainer;
    private SpellEffectsController _spellEffects;
    private Label _lastSpellText;
    private string _lastUsedSpell;

    public override void _Ready()
    {
        _spellTree = new SpellTree();
        _currSequence = new List<StringName>();
        _spellContainer = new Node3D();
        AddChild(_spellContainer);
        _spellIconsContainer = GetNode<VBoxContainer>("SpellIcons/SpellIconsContainer");
        _learnedSpellsContainer = GetNode<VBoxContainer>("LearnedSpells/LearnedSpellsContainer");
        _spellEffects = GetParent().GetNode<SpellEffectsController>("SpellEffectsController");
        _lastSpellText = GetNode<Label>("CastedSpell");
    }

    public override void _Process(double delta)
    {
        CheckForInput();
    }

    private void CheckForInput()
    {
        //todo find a better way to handle that
        if (Input.IsActionJustPressed(SpellNames.Cancel))
        {
            GD.Print("Canceled casting");
            ClearSequence();
        }
        if (Input.IsActionJustPressed(SpellNames.Fire))
        {
            GD.Print("Casted Fire");
            HandleCasting(SpellNames.Fire, "Fire");
        }
        else if (Input.IsActionJustPressed(SpellNames.Water))
        {
            GD.Print("Casted Water");
            HandleCasting(SpellNames.Water, "Water");
        }
        else if (Input.IsActionJustPressed(SpellNames.Lightning))
        {
            GD.Print("Casted Lightning");
            HandleCasting(SpellNames.Lightning, "Lightning");
        }
        else if (Input.IsActionJustPressed(SpellNames.Holy))
        {
            GD.Print("Casted Holy");
            HandleCasting(SpellNames.Holy, "Holy");
        }
        else if (Input.IsActionJustPressed(SpellNames.Dark))
        {
            GD.Print("Casted Dark");
            HandleCasting(SpellNames.Dark, "Dark");
        }
        else if (Input.IsActionJustPressed(SpellNames.Projectile))
        {
            GD.Print("Casted Projectile");
            HandleCasting(SpellNames.Projectile, "Projectile");
        }
        else if (Input.IsActionJustPressed(SpellNames.Aoe))
        {
            GD.Print("Casted Aoe");
            HandleCasting(SpellNames.Aoe, "Aoe");
        }
        else if (Input.IsActionJustPressed(SpellNames.Beam))
        {
            GD.Print("Casted Beam");
            HandleCasting(SpellNames.Beam, "Beam");
        }
        else if (Input.IsActionJustPressed(SpellNames.Buff))
        {
            GD.Print("Casted Buff");
            HandleCasting(SpellNames.Buff, "Buff");
        }
        if (Input.IsActionJustPressed("mouse_right"))
            CheckForSpell();
    }

    private void HandleCasting(StringName spell_name, string spell_text)
    {
        _currSequence.Add(spell_name);
        Label label = new Label
        {
            Text = spell_text
        };
        _spellIconsContainer.AddChild(label);
    }

    /// <summary>
    /// checks if the entered sequence corresponds to a known spell
    /// </summary>
    private void CheckForSpell()
    {
        string spell = _spellTree.GetSpell(_currSequence);
        if (spell != null)
        {
            GD.Print("Casting " + spell);
            _lastSpellText.Text = spell;
            _spellEffects.Cast(spell);
            BindSpell(spell);
        } 
        // if the player casts a spell with no input sequence, cast the last used spell
        else if (_currSequence.Count == 0) 
        {
            _spellEffects.Cast(_lastUsedSpell);
        }
        ClearSequence();
    }

    /// <summary>
    /// clears the input
    /// </summary> <summary>
    private void ClearSequence()
    {
        _currSequence.Clear();
        foreach (var child in _spellIconsContainer.GetChildren())
        {
            child.QueueFree();
        }
    }

    /// <summary>
    /// remembers the last casted spell
    /// </summary>
    /// <param name="spell"></param>
    private void BindSpell(string spell)
    {
        _lastUsedSpell = spell;
    }
}
