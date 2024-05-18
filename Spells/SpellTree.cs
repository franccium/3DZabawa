using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// node in the tree that contains its' assigned spell and the key that grants access to the further nodes
/// </summary>
public class SpellNode
{
    public string SpellName;
    public Dictionary<StringName, SpellNode> NextSpell { get; set; } = new Dictionary<StringName, SpellNode>();
}

public partial class SpellTree : Node
{
    private SpellNode _root;

    public SpellTree()
    {
        _root = new SpellNode();

        AddSpell(new List<StringName> { SpellNames.Fire }, "Fireball");
        AddSpell(new List<StringName> { SpellNames.Fire, SpellNames.Aoe }, "FirePillar");
        AddSpell(new List<StringName> { SpellNames.Dark, SpellNames.Aoe }, "DarkPillar");
        AddSpell(new List<StringName> { SpellNames.Lightning, SpellNames.Water, SpellNames.Aoe }, "ElectricityField");
        AddSpell(new List<StringName> { SpellNames.Holy, SpellNames.Beam }, "HolyBeam");
        AddSpell(new List<StringName> { SpellNames.Lightning, SpellNames.Buff }, "SpeedBuff");
    }

    /// <summary>
    /// adds a new spell with a new key sequence and a name (name must match the one in skilleffectcontroller.cs)
    /// </summary>
    /// <param name="key_sequence"></param>
    /// <param name="spell_name"></param>
    private void AddSpell(List<StringName> key_sequence, string spell_name)
    {
        SpellNode currNode = _root;
        foreach (var key in key_sequence)
        {
            if (!currNode.NextSpell.ContainsKey(key))
            {
                // if the combination dosent exist, create a new spell with this combination as a new brach
                currNode.NextSpell[key] = new SpellNode();
            }
            // go to the next branch
            currNode = currNode.NextSpell[key];
        }
        // assign a new spell node to the tree
        currNode.SpellName = spell_name;
    }

    /// <summary>
    /// checks if the sequence is valid and returns the spell hidden under it
    /// </summary>
    /// <param name="key_sequence"></param>
    /// <returns></returns>
    public string GetSpell(List<StringName> key_sequence)
    {
        SpellNode currNode = _root;
        int inputIndex = 0;
        foreach (var key in key_sequence)
        {
            GD.Print("checking for " + key);
            foreach (var keys in currNode.NextSpell.Keys)
            {
                GD.Print("next combination " + keys);
            }
            if (currNode.NextSpell.ContainsKey(key))
            {
                currNode = currNode.NextSpell[key];
                GD.Print("contains " + key);
                inputIndex++;
            }
            else if (!currNode.NextSpell.ContainsKey(key))
            {
                GD.Print("Invalid Sequence!");
                currNode = _root;
                inputIndex = 0;
                break;
            }
        }

        if (inputIndex == key_sequence.Count)
            return currNode.SpellName;
        else
            return null;
    }
}
