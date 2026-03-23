
# 1. Overview
## InteractionLocker
`InteractionLocker` is a system that lets you temporarily **disable or block any kind of gameplay interaction in Unity**.
<br><br>
It provides a centralized way to control what the player can interact with — **without using extra colliders, invisible blockers, or UI overlays**.
<br/><br/>
The core idea is simple: the system manages a set of logical interaction layers and calculates the resulting `InteractionLayerMask` and the corresponding `LayerMask`.

### When this is useful
- Opening a menu and disabling character controls
- Showing a tutorial and blocking scene or UI interactions
- Replacing messy “click interceptors” with a clean, unified solution
`InteractionLocker` handles these cases consistently and in one place.

### What you can do with it

- disable components based on an interaction layer
- update the `LayerMask` used by `PhysicsRaycaster`
- apply the current blocked‑layer mask to any raycast
- enable/disable gameplay systems depending on interaction state
- and more

### How it works
`IInteractionLockerService` keeps track of which interaction layers are currently blocked (`InteractionLayerMask`) and calculates the matching `LayerMask`.<br>
Any component can request the current mask or subscribe to updates to automatically react when layer availability changes.<br>
The system supports multiple simultaneous locks.<br>
A layer becomes unlocked only when all sources release their lock.<br>

## InteractionLayer
To simplify usage, the system introduces abstract interaction layers — `InteractionLayer`.<br>
These logical layers let you group multiple Unity layers under a single name and manage them as one unit.<br>
For example, you might define layers like Game, UI, Tutorial, Dialogue, each mapping to its own `LayerMask`.<br>

### Why InteractionLayer exists
- to group several Unity layers under one interaction layer (or not use Unity layers at all)
- to disable components that don’t rely on LayerMask (e.g., GraphicRaycaster, character controllers)
- to provide an abstraction layer that unifies different interaction types under a single concept

# 2. Setup

## InteractionLayer
### InteractionLayersAsset
For the system to work with interaction layers, you first need to create a shared container — **`InteractionLayersAsset`**.  
It stores the full list of logical interaction layers used in your project.

<img width="223" height="95" alt="InteractionLayersAsset" src="https://github.com/user-attachments/assets/c73b41dd-4a80-4d8c-a4d3-521b12d60591" /><br>
<img width="409" height="200" alt="InteractionLayersAsset" src="https://github.com/user-attachments/assets/c1c77d79-cdbf-4747-ba75-fab841be5965" />

- Create an `InteractionLayersAsset` (`ScriptableObject`).
- Add one or more `InteractionLayerAsset` entries to it.


### InteractionLayerAsset

`InteractionLayerAsset` contains all the data required to describe a logical interaction layer:
<img width="410" height="147" alt="InteractionLayerAsset" src="https://github.com/user-attachments/assets/48f4269d-2b44-4370-817a-775b51b96c1f" /><br>
Each asset defines:
- a **unique bit** in the global interaction mask  
- a set of Unity Layers that will be included in the resulting `LayerMask` when this layer is blocked  

## IInteractionLockerService

`InteractionLockerService` is the core of the system.  
It tracks which interaction layers are currently locked and recalculates the resulting masks.

### Creating the service

To create the service, you need a prepared `InteractionLayersAsset`:

```csharp
private InteractionLayersAsset _layerConfig;

public void InitializeLocker()
{
    IInteractionLockerService locker = InteractionLockerService.Create(_layerConfig);
}
```

### Accessing the service
Once created, the service can be accessed in any way that fits your architecture:  
via dependency injection, as a singleton, or through a service locator.<br>

### Locking and unlocking layers
A layer is locked by calling `Lock` and passing either a single layer or a set of layers.  
The method returns an `InteractionLockHandle`. Releasing this handle (via `Dispose`) removes the lock.<br><br>
`InteractionLockHandle` is safe to copy — every copy remains valid and always reflects the current state.  
It’s implemented as a struct with zero allocations, so using it does not generate garbage.


# 3. Usage
The repository includes a demonstration scene called **SampleScene** and several scripts showcasing how the service works.
### Basic example

```csharp
private IInteractionLockerService _service;
[SerializeField] private InteractionLayerAsset _layer;

public void Example()
{
    // Lock an interaction layer using InteractionLayerAsset
    var handle = _service.Lock(_layer);

    // Alternative — lock using the InteractionMask directly
    var handle = _service.Lock(_layer.InteractionMask);

    // Release the lock
    handle.Dispose();

    // Get the current mask state
    var state = _service.LockData.Value;
    InteractionLayerMask interactionMask = state.InteractionMask;
    LayerMask layerMask = state.LayerMask;
}
```

### Async usage

```csharp
public async UniTask ExampleAsync()
{
    // The layer stays locked for the duration of the async operation
    using (_service.Lock(_layer))
        await DoSomethingAsync();
}
```

### Updating PhysicsRaycaster mask

Updating the `PhysicsRaycaster` mask by excluding blocked layers.

```csharp
public sealed class RaycasterMaskUpdater : MonoBehaviour
{
    private LayerMask _originalMask;
    private IInteractionLockerService _service;

    [SerializeField] private PhysicsRaycaster _raycaster;

    private void Awake()
    {
        // Store the original mask
        _originalMask = _raycaster.eventMask;

        // Subscribe to mask updates
        _service.MaskChanged += OnMaskChanged;
    }

    private void OnMaskChanged(InteractionLockData data)
    {
        // Remove blocked layers from the original mask
        var blocked = data.LayerMask;
        var updated = _originalMask & ~blocked;

        _raycaster.eventMask = updated;
    }
}
```

### Applying the mask to a Raycast

```csharp
private void ExampleRaycast(Vector3 origin, Vector3 direction, float distance, LayerMask baseMask)
{
    var state = _service.Mask;

    // Remove blocked layers
    var blocked = state.LayerMask;
    var finalMask = baseMask & ~blocked;

    var hit = Physics2D.Raycast(origin, direction, distance, finalMask);
}
```

<br><br>
---
<br><br>
# 1. Описание

## InteractionLocker
`InteractionLocker` — система, которая позволяет временно **отключать/блокировать любые игровые взаимодействия в Unity**.
<br/><br/>
Она позволяет централизованно управлять тем, с чем игрок может взаимодействовать — **без перекрывающих коллайдеров и вспомогательных UI‑слоёв.
<br/><br/>
Принцип работы основан на управлении и расчёте текущей маски интерактивных слоёв включая соотвествующий им  `LayerMask`.

### Для чего это нужно :
- при открытии меню - отключить управление персонажем
- при показе туториала - отключить взаимодействие со сценой и UI
- когда обычные "перехватчики кликов" становятся запутанными и неудобными
`InteractionLocker` решает эти задачи единообразно и централизованно.

### Что позволяет делать :
- отключать управляющие компоненты по указанному интерактивному слою 
- изменять `LayerMask` у `PhysicsRaycaster`
- применять текущую маску заблокированных слоёв (`LayerMask`) при использовании raycast'ов
- изменение состояния управляющих объектов.
- и т.д.
  
### Как это работает
`IInteractionLockerService` хранит текущее состояние заблокированных слоёв `InteractionLayerMask` и вычисляет соотвествующий им `LayerMask`.<br>
Любой компонент может запросить актуальную маску или подписаться на её изменения, чтобы автоматически реагировать на изменение доступности слоёв.<br>
Система поддерживает несколько одновременных блокировок, и слой считается разблокированным только после того, как все источники сняли свои ограничения.<br>

## InteractionLayer
Чтобы упростить работу, система использует абстрактные слои взаимодействий - `InteractionLayer`.<br>
Эти логические слои позволяют объединить под одним именем несколько `UnityLayer` и управлять и доступностью как единой группой.<br>
Например можно создать слои Game, UI, Tutorial, Dialogue, каждый из которых может отвечать со свою маску `LayerMask`.<br>

### Для чего нужны InteractionLayer:
- позволяют группировать несколько значений `LayerMask` под одним интерактивным слоем, или не привязываться к ним вовсе
- для отключения компонентов, которые не работают или не привязаны к `LayerMask` (например, GraphicRaycaster или контроллер персонажа)
- создают дополнительный уровень абстракции, позволяющий объединять разные взаимодействия под одним названием

# 2.Настройка

## InteractionLayer
### InteractionLayersAsset
Чтобы система могла работать со слоями взаимодействий, сначала нужно создать общий контейнер — `InteractionLayersAsset`.
Он хранит список всех логических слоёв взаимодействий, которые будут использоваться в проекте.

<img width="223" height="95" alt="InteractionLayersAsset" src="https://github.com/user-attachments/assets/c73b41dd-4a80-4d8c-a4d3-521b12d60591" /><br>
<img width="409" height="200" alt="InteractionLayersAsset" src="https://github.com/user-attachments/assets/c1c77d79-cdbf-4747-ba75-fab841be5965" />

- Создайте `InteractionLayersAsset` (`ScriptableObject`).
- Добавьте в него один или несколько I`nteractionLayerAsset`.

### InteractionLayerAsset
`InteractionLayerAsset` содержит всю необходимую для работы информацию о логическом слое: <br><br>
<img width="410" height="147" alt="InteractionLayerAsset" src="https://github.com/user-attachments/assets/48f4269d-2b44-4370-817a-775b51b96c1f" /><br>
- свой **уникальный** бит в общей маске
- связанные с `Unity Layers`, которые попадут в итоговую маску при блокировке этого слоя

## IInteractionLockerService
`InteractionLockerService` — это центральный сервис системы.
Он хранит текущее состояние блокировок и пересчитывает итоговую маску слоёв.

### Как создать сервис
Для создания сервиса нужен заранее подготовленный `InteractionLayersAsset`:
```cs
private InteractionLayersAsset _layerConfig;

public void InitializeLocker()
{
    IInteractionLockerService locker = InteractionLockerService.Create(_layerConfig);
}
```
### Доступ к сервису
После создания к сервису можно получать доступ любым удобным способом: через DI, как к синглтону или через сервис‑локатор.<br>

### Блокировка/отключение слоёв
Блокировка слоя выполняется через вызов метода `Lock`, которому можно передать один слой или набор слоёв.<br>
Метод возвращает структуру `InteractionLockHandle`. Когда этот хэндл освобождается (через `Dispose`), блокировка снимается.
<br><br>
`InteractionLockHandle` можно безопасно копировать — каждая копия остаётся валидной и содержит актуальное состояние.<br>
Хэндл реализован как структура без аллокаций, поэтому работа с ним не создаёт лишнего мусора в памяти.

# 3.Использование

В репозитории есть демонстрационная сцена **SampleScene** и несколько скриптов, показывающих работу сервиса.

### Базовый пример
```cs
private IInteractionLockerService _service;
[SerializeField] private InteractionLayerAsset _layer;

public void Example()
{
    // Заблокировать интерактивный слой используя InteractionLayerAsset
    var handle = _service.Lock(_layer);

    // Альтернативный вариант — заблокировать используя InteractionMask.
    var handle = _service.Lock(_layer.InteractionMask);

    // Cнять/отменить блокировку
    handle.Dispose();

    // Получить текущее состояние масок
    var state = _service.LockData.Value;
    InteractionLayerMask interactionMask = state.InteractionMask;
    LayerMask layerMask = state.LayerMask;
}
```
### Асинхронное использование
```cs
public async UniTask ExampleAsync()
{
    // Блокировка действует, пока выполняется асинхронная операция
    using (_service.Lock(_layer))
        await DoSomethingAsync();
}
```
### Модификация маски PhysicsRaycaster
Обновлении маски у PhysicsRaycaster, исключая заблокированные слои.
```cs
public sealed class RaycasterMaskUpdater : MonoBehaviour
{
    private LayerMask _originalMask;
    private IInteractionLockerService _service;

    [SerializeField] private PhysicsRaycaster _raycaster;

    private void Awake()
    {
        // Сохраняем исходную маску
        _originalMask = _raycaster.eventMask;

        // Подписываемся на изменения маски
        _service.MaskChanged += OnMaskChanged;
    }

    private void OnMaskChanged(InteractionLockData data)
    {
        // Исключаем заблокированные слои из исходной маски
        var blocked = data.LayerMask;
        var updated = _originalMask & ~blocked;

        _raycaster.eventMask = updated;
    }
}
```
### Применение маски для Raycast
```cs
private void ExampleRaycast(Vector3 origin, Vector3 direction, float distance, LayerMask baseMask)
{
    var state = _service.Mask;

    // Исключаем заблокированные слои
    var blocked = state.LayerMask;
    var finalMask = baseMask & ~blocked;

    var hit = Physics2D.Raycast(origin, direction, distance, finalMask);
}
```
