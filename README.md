# Dddify Admin

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4.svg)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-19-61DAFB.svg)](https://react.dev/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-ready-4169E1.svg)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-ready-DC382D.svg)](https://redis.io/)

Dddify Admin 是一个基于 **ASP.NET Core + React** 的现代化中后台管理系统模板。后端采用 DDD 与 Clean Architecture 分层，前端基于 Umi Max、Ant Design 与 Ant Design Pro 构建，内置认证授权、用户、角色、权限、部门、数据字典等常见后台能力，适合作为企业级管理系统的基础工程。

## 特性

- DDD 分层架构：Domain、Application、Infrastructure、Web 职责清晰
- CQRS 风格用例组织：Command 处理写操作，Query 处理读操作
- JWT 认证与刷新令牌机制
- 基于角色与权限码的授权控制
- 用户、角色、权限、部门、数据字典等基础模块
- PostgreSQL + EF Core 数据持久化
- Redis 缓存、分布式锁与会话辅助能力
- OpenAPI + Scalar API 文档
- React 19、TypeScript、Umi Max、Ant Design Pro 前端工程
- 中英文国际化基础支持

## 技术栈

### 后端

- .NET 10
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- Redis
- OpenAPI / Scalar
- Dddify

### 前端

- React 19
- TypeScript 6
- Umi Max 4
- Ant Design 6
- Ant Design Pro Components
- TanStack Query
- Tailwind CSS
- Biome
- Jest

## 项目结构

```text
dddify-admin
├── src
│   ├── Dddify.Admin.Domain
│   │   ├── Aggregates
│   │   ├── Events
│   │   ├── Exceptions
│   │   └── Repositories
│   ├── Dddify.Admin.Application
│   │   ├── Abstractions
│   │   ├── Caching
│   │   ├── Commands
│   │   ├── Configuration
│   │   ├── Queries
│   │   ├── Dtos
│   │   ├── Events
│   │   └── Exceptions
│   ├── Dddify.Admin.Infrastructure
│   │   ├── Announcements
│   │   ├── Authentication
│   │   ├── BackgroundServices
│   │   │   └── Sessions
│   │   ├── Caching
│   │   ├── Email
│   │   ├── Identifiers
│   │   ├── Locking
│   │   ├── Persistence
│   │   │   ├── EntityConfigurations
│   │   │   ├── Migrations
│   │   │   ├── Repositories
│   │   │   └── ApplicationDbContext.cs
│   │   ├── Security
│   │   └── Timing
│   └── Dddify.Admin.Web
│       ├── Authentication
│       ├── Authorization
│       ├── Controllers
│       ├── Requests
│       ├── Resources
│       └── ClientApp
├── tests
│   ├── Dddify.Admin.Application.Tests
│   ├── Dddify.Admin.Domain.Tests
│   └── Dddify.Admin.IntegrationTests
├── Dddify.Admin.slnx
├── LICENSE
└── README.md
```

## 架构分层

| 层 | 职责 | 典型内容 |
| --- | --- | --- |
| Domain | 表达核心领域模型与业务规则 | 聚合、实体、值对象、领域事件、领域异常、仓储接口 |
| Application | 编排业务用例 | Command、Query、DTO、应用异常、应用服务接口 |
| Infrastructure | 承载技术实现 | EF Core、Redis、仓储实现、认证、缓存、分布式锁 |
| Web | 提供交付入口 | Controller、Request、认证授权、OpenAPI、前端应用 |

依赖方向保持由外向内：Web 依赖 Application 和 Infrastructure，Infrastructure 依赖 Application 和 Domain，Domain 不依赖外部技术实现。

`Application.Abstractions` 按能力组织接口及配套契约模型，Infrastructure 使用对应的能力目录组织实现，例如 `Abstractions.Email` 对应 `Infrastructure.Email`。持久化实现统一放在 `Persistence`，会话清理等托管服务放在 `BackgroundServices`，连接参数与技术配置保留在所属实现模块中。

## 内置模块

- 认证授权：账号登录、刷新令牌、退出登录
- 会话管理：刷新令牌、会话撤销、会话清理
- 用户管理：用户维护、状态管理、角色分配、密码重置
- 角色管理：角色维护、默认角色、内置角色、权限分配
- 权限管理：目录、菜单、按钮三级权限模型
- 部门管理：树形部门、负责人、部门类型、状态管理
- 数据字典：字典、字典项、排序、预设项、启用状态管理

## 环境要求

- .NET SDK 10.0+
- Node.js 20.0+
- PostgreSQL
- Redis
- Docker，可选，用于本地快速启动依赖服务

## 快速开始

### 1. 克隆项目

```bash
git clone https://github.com/esofar/dddify-admin.git
cd dddify-admin
```

### 2. 启动依赖服务

```bash
docker run -d \
  --name dddify-admin-postgres \
  --restart unless-stopped \
  -e POSTGRES_USER=app_user \
  -e POSTGRES_PASSWORD=app_pwd \
  -e POSTGRES_DB=dddify_admin \
  -p 5432:5432 \
  -v dddify_admin_postgres_data:/var/lib/postgresql/data \
  postgres:17
```

```bash
docker run -d \
  --name dddify-admin-redis \
  --restart unless-stopped \
  -p 6379:6379 \
  -v dddify_admin_redis_data:/data \
  redis:8.6 \
  redis-server --appendonly yes --requirepass app_pwd
```

如果容器已存在，可以直接启动：

```bash
docker start dddify-admin-postgres dddify-admin-redis
```

### 3. 配置后端

后端配置文件位于：

```text
src/Dddify.Admin.Web/appsettings.json
```

默认开发配置示例：

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=dddify_admin;Username=app_user;Password=app_pwd"
  },
  "Redis": {
    "Configuration": "localhost:6379,password=app_pwd,abortConnect=false",
    "InstanceName": "dddify:"
  },
  "Jwt": {
    "Secret": "YourVeryStrongAndLongSecretKeyHere1234567890ABCDEF",
    "Issuer": "your_issuer",
    "Audience": "your_audience",
    "AccessTokenMinutes": 15,
    "RefreshTokenDays": 7
  }
}
```

生产环境请使用环境变量、密钥管理服务或部署平台 Secret 覆盖敏感配置。

### 4. 初始化数据库

```powershell
dotnet ef database update `
  --project src/Dddify.Admin.Infrastructure/Dddify.Admin.Infrastructure.csproj `
  --startup-project src/Dddify.Admin.Web/Dddify.Admin.Web.csproj `
  --context ApplicationDbContext
```

新增迁移示例：

```powershell
dotnet ef migrations add InitialCreate `
  --project src/Dddify.Admin.Infrastructure/Dddify.Admin.Infrastructure.csproj `
  --startup-project src/Dddify.Admin.Web/Dddify.Admin.Web.csproj `
  --context ApplicationDbContext `
  --output-dir Persistence/Migrations
```

### 5. 启动后端

```bash
dotnet run --project src/Dddify.Admin.Web/Dddify.Admin.Web.csproj
```

开发环境启动后可以访问 API 文档：

```text
https://localhost:7225/docs
```

实际端口以 `launchSettings.json` 或控制台输出为准。

### 6. 启动前端

```bash
cd src/Dddify.Admin.Web/ClientApp
npm install
npm run dev
```

前端开发服务会通过 Umi 配置代理访问后端 API。

## 常用命令

### 后端

```bash
dotnet restore
dotnet build
dotnet test
```

### 前端

```bash
cd src/Dddify.Admin.Web/ClientApp

npm run dev
npm run build
npm run lint
npm run tsc
npm run biome
npm run test
npm run openapi
```

## 配置说明

### JWT

| 配置项 | 说明 |
| --- | --- |
| `Secret` | Token 签名密钥 |
| `Issuer` | 签发方 |
| `Audience` | 接收方 |
| `AccessTokenMinutes` | 访问令牌有效期，单位分钟 |
| `RefreshTokenDays` | 刷新令牌有效期，单位天 |

### Redis

| 配置项 | 说明 |
| --- | --- |
| `Configuration` | Redis 连接字符串 |
| `InstanceName` | 缓存 Key 前缀 |

Redis 当前用于缓存、分布式锁、会话辅助等场景。

### 邮箱登录

系统使用邮箱作为账号，支持密码登录和邮箱验证码登录：

- `POST /api/v1/auth/login/password`：提交 `email` 和密码。
- `POST /api/v1/auth/login/verification-code/send`：提交 `email`，发送六位登录验证码。
- `POST /api/v1/auth/login/verification-code`：提交 `email`、`code` 和设备信息完成登录。

验证码默认 5 分钟有效、60 秒发送冷却，每个邮箱每小时最多发送 5 次；连续输错 5 次失效，成功验证后立即消费。Redis 仅存验证码的 HMAC 摘要。未注册或禁用的邮箱不会发送邮件，接口统一返回发送请求已处理。

`EmailVerificationCode` 配置验证码有效期、限流及 `HashSecret`。生产环境通过 `EmailVerificationCode__HashSecret` 设置至少 32 个字符的独立随机密钥。

开发环境默认启用 `Email:UseDevelopmentSender`，邮件及验证码输出到后端日志，便于本地联调。实际发送邮件时将其设为 `false`，配置以下 SMTP 参数：

```json
{
  "Email": {
    "UseDevelopmentSender": false,
    "Host": "smtp.example.com",
    "Port": 587,
    "EnableSsl": true,
    "UserName": "noreply@example.com",
    "Password": "",
    "From": "noreply@example.com",
    "DisplayName": "Dddify Admin"
  }
}
```

实际邮件通过 MailKit 发送。`EnableSsl: true` 时，端口 465 使用隐式 TLS，其他端口使用强制 STARTTLS（通常为 587）；服务器不支持 STARTTLS 时发送失败，不会降级为明文。`EnableSsl: false` 用于无需 TLS 的本地 SMTP 服务。SMTP 密码通过 `Email__Password` 等环境变量或 Secret 配置。开发日志发送器仅允许在 Development 环境启用。

手机短信登录接口、短信供应商 SDK 及 `Sms` 配置已移除；用户手机号保留为联系方式。

## 初始数据

项目包含基础种子数据：

- 内置角色：超级管理员、管理员、普通用户、访客
- 权限树：系统、用户、角色、权限、部门、字典等模块权限
- 部门树：集团、产品中心、技术中心、职能中心及下属部门
- 数据字典：部门类型等基础字典
- 测试用户：一组企业通讯录风格的测试用户

超级管理员账号：

```text
邮箱账号：chengyuan.gu@xinghan.tech
初始密码：Admin123
```

生产环境初始化后请立即修改默认账号密码。

## 开发约定

### 后端

- 领域规则放在 Domain 层，避免在 Controller 中堆积业务逻辑
- Command 处理写操作，Query 处理读操作
- Controller 只负责请求转换、授权校验和调用应用层
- Infrastructure 层承载 EF Core、Redis、认证等技术实现
- 应用异常表达用例失败，领域异常表达领域规则被破坏
- 缓存、分布式锁、邮件等横切能力通过接口抽象
- 种子数据应保持稳定 ID，避免迁移文件反复抖动

### 前端

- 页面优先使用 ProTable、ModalForm、DrawerForm 等 Pro Components
- 接口调用统一使用 `src/services/v1` 下的服务
- 表格请求统一适配 `{ data, total, success }`
- 成功与失败提示优先使用已有国际化 Key
- 权限判断使用 `useAccess`
- 枚举展示统一使用 `valueEnum`、Tag 或业务组件
- 表单组件保持类型安全，避免无意义的 `any`
- 国际化文案同时维护 `zh-CN` 与 `en-US`

## API 文档

后端集成 OpenAPI 与 Scalar。开发环境启动 Web 项目后访问：

```text
https://localhost:7225/scalar
```

也可以通过 OpenAPI 文档生成前端服务：

```bash
cd src/Dddify.Admin.Web/ClientApp
npm run openapi
```

## 测试

运行全部 .NET 测试：

```bash
dotnet test
```

运行前端测试：

```bash
cd src/Dddify.Admin.Web/ClientApp
npm run test
```

## 部署建议

- 使用环境变量或 Secret 管理数据库、Redis、JWT 等敏感配置
- 生产环境关闭敏感数据日志
- 启用 HTTPS
- 为 PostgreSQL 和 Redis 配置备份策略
- 多实例部署时，后台任务需要具备互斥能力
- 为关键操作保留审计日志
- 根据实际网关或反向代理配置 `ForwardedHeaders`

## 贡献

欢迎提交 Issue 和 Pull Request。提交代码前建议先运行：

```bash
dotnet test
cd src/Dddify.Admin.Web/ClientApp
npm run lint
npm run test
```

## 许可证

本项目基于仓库内 [LICENSE](./LICENSE) 文件授权。
