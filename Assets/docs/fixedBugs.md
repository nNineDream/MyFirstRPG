# BUGs

**2026/2/6**
- **跳跃时有可能一直卡在jumpstate**
reason：jumpstate->fallstate的Vy<0条件有可能在Vy=0时一直不成立（比如角色头上有方块顶住，一开始就无法正确赋予Vy）
solve：修改jumpstate->fallstate的变化条件为Vy<=0
- **蹬墙跳手感奇怪，跳跃后方向固定，操纵感差。无法做到连续攀爬一面墙壁这样的操作**

