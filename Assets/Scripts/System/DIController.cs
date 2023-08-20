using System;
using UnityEngine;

public class DIController : MonoBehaviour
{
    [SerializeReference] private CellSlotsFactory _factory = null;
    [SerializeReference] private ChipFactory _chipFactory;
    [SerializeReference] private CoverFactory _coverFactory;
    [SerializeReference] private GeneratorFactory _generatorFactory;
    private void Awake()
    {
        _factory.Construct(_chipFactory, _coverFactory, _generatorFactory);
    }
}