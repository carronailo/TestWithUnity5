using UnityEngine;

/// <summary>
/// InputDetector族代码类用来读取Unity引擎产生的原始输入数据（Input类），Unity引擎内部时钟的执行顺序是：0~N次的FixedUpdate->Update->LateUpdate，如此循环，
/// 如果调整过脚本的执行顺序，那么所造成的影响是每一次执行FixedUpdate或Update或LateUpdate的时候，会按照定义的顺序挨个脚本的执行，但是并不会影响整体循环的
/// 执行顺序。Unity引擎的原始输入数据的介入时机是在时钟循环的间隔处，可以认为是：每一次循环开始时更新输入数据，此数据记录上一次循环的时间片里所接收到的
/// 物理输入；也可以认为是：每一次循环结束时更新输入数据，此数据记录当前循环的时间片内所接收到的物理输入。对于脚本处理来说这两种理解是一模一样的。换句话
/// 说，每一次循环内，无论是在FixedUpdate中取输入数据还是在Update或者LateUpdate中获取输入数据，所得到的结果不会有变化。
/// 基于上述引擎时钟原理，理论上说无论是在FixedUpdate中处理Unity引擎的原始输入数据，还是在Update又或者是LateUpdate中，对于输入数据的处理本身没有什么区别，
/// 区别在于由数据变化所驱动的脚本逻辑的执行结果会因为处理数据的时机不同而发生改变。比如说：脚本逻辑的执行发生在FixedUpdate，如果对输入数据的处理放在了
/// Update中的话，那么脚本逻辑的执行在反映输入变化的层面上会永远延迟一帧。但是，脚本逻辑放在何处是由具体的逻辑内容决定的（一般遵循的规律：影响渲染的逻辑
/// 在Update，影响物理的逻辑在FixedUpdate，影响摄像机的逻辑在LateUpdate），另外，FixedUpdate本身执行的不稳定性（并不确保一帧里执行一次），这些因素又会让
/// 脚本逻辑的执行和输入数据的处理之间的关系更加复杂。
/// 基于InputSystem的设计目的（将Unity引擎的原始输入数据转化为游戏逻辑能够直接使用的逻辑输入数据），将其实现为一个与Unity引擎下Input类齐平的逻辑输入数据
/// 提供模块。针对Unity引擎通过Input提供的各类原始输入数据，设计一族原始输入数据读取单元（InputDetector）来将引擎的原始输入数据采集汇总到InputSystem中来。
/// 考虑到上述引擎时钟的问题，InputDetector的数据采集会在每一次的Update和FixedUpdate中都进行，并且每一次进行采集都会向InputSystem上报。同时，设计一族将原
/// 始输入数据转换为逻辑输入数据的逻辑输入数据提供单元（InputModule）来将汇总到InputSystem的原始输入数据分门别类转换为逻辑输入数据并提供其他逻辑模块使用的
/// 数据访问接口。
/// </summary>
public abstract class InputDetector : MonoBehaviour
{
	
}
