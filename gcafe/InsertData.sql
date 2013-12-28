/*
后期部署脚本模板							
--------------------------------------------------------------------------------------
 此文件包含将附加到生成脚本中的 SQL 语句。		
 使用 SQLCMD 语法将文件包含到后期部署脚本中。			
 示例:      :r .\myfile.sql								
 使用 SQLCMD 语法引用后期部署脚本中的变量。		
 示例:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

SET IDENTITY_INSERT [dbo].[branch] ON 
INSERT [dbo].[branch] ([id], [name]) VALUES (1, N'总店')
INSERT [dbo].[branch] ([id], [name]) VALUES (2, N'江南西分店')
INSERT [dbo].[branch] ([id], [name]) VALUES (3, N'莱茵分店')
INSERT [dbo].[branch] ([id], [name]) VALUES (4, N'区庄分店')
INSERT [dbo].[branch] ([id], [name]) VALUES (5, N'潮流分店')
SET IDENTITY_INSERT [dbo].[branch] OFF

SET IDENTITY_INSERT [dbo].[role] ON 
INSERT [dbo].[role] ([id], [name]) VALUES (1, N'系统管理员')
INSERT [dbo].[role] ([id], [name]) VALUES (2, N'经理')
INSERT [dbo].[role] ([id], [name]) VALUES (3, N'收银')
INSERT [dbo].[role] ([id], [name]) VALUES (4, N'点菜')
INSERT [dbo].[role] ([id], [name]) VALUES (5, N'服务员')
SET IDENTITY_INSERT [dbo].[role] OFF

SET IDENTITY_INSERT [dbo].[staff] ON 
INSERT [dbo].[staff] ([id], [number], [name], [password], [branch_id], [role_id], [join_date], [is_deleted]) VALUES (1, N'01001', N'赖家宁', N'laijianing', 1, 1, CAST(0xE6370B00 AS Date), 0)
INSERT [dbo].[staff] ([id], [number], [name], [password], [branch_id], [role_id], [join_date], [is_deleted]) VALUES (2, N'01002', N'张志强', N'zhangzq71', 1, 1, CAST(0xE6370B00 AS Date), 0)
SET IDENTITY_INSERT [dbo].[staff] OFF

SET IDENTITY_INSERT [dbo].[printer_group] ON
INSERT [dbo].[printer_group] ([id], [branch_id], [name]) VALUES (1, 1, N'厨房组');
SET IDENTITY_INSERT [dbo].[printer_group] OFF

SET IDENTITY_INSERT [dbo].[menu_catalog] ON
INSERT [dbo].[menu_catalog] ([id], [name]) VALUES (1, N'厨房菜牌');
INSERT [dbo].[menu_catalog] ([id], [name]) VALUES (2, N'酒吧菜牌');
SET IDENTITY_INSERT [dbo].[menu_catalog] OFF

SET IDENTITY_INSERT [dbo].[device] ON 
INSERT [dbo].[device] ([id], [device_id], [register_ticket], [register_time], [is_deny]) VALUES (1, N'Ne66UXi2gD5kBlIDD0LVFRlUWs8=', N'Ne66UXi2gD5kBlIDD0LVFRlUWs8=', CAST(0x0000A29900000000 AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[device] OFF

SET IDENTITY_INSERT [dbo].[sys_info] ON 
INSERT [dbo].[sys_info] ([id], [branch_id], [order_cnt], [is_festival]) VALUES (1, 1, 0, 0)
SET IDENTITY_INSERT [dbo].[sys_info] OFF