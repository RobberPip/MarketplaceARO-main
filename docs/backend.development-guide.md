# Backend Development Guide

[Черновик идей и правил которыми хотелось бы руководствоваться](https://www.figma.com/board/aMm98lWMvN3p5VruBGMgXU/%D0%A1%D0%9E%D0%9B%D0%98%D0%94-%D0%A5%D0%A3%D0%81%D0%9B%D0%98%D0%94.-Draft?node-id=1-726&t=eJ4LhMEvZIsO1G2t-4).  
Он будет пополнятся и переделываться (нет).

> [!NOTE]  
> TODO: перенести тезисы сюда.

## Не надо множить сущности и модули

На данный момент предлагаю думать что у нас ровно один модуль (назовем его Market), который будет содержать в себе всю логику по проекту.

Выносить `UserProfileModule` в отдельный модуль - нецелесообразно.  
`UserProfile` это корневая сущность нашего приложения, в отличии от обвязки к Identity, которая потенциально может быть заменена.

## Не делаем абстракции на про запас

Яркий пример - репозитории. На данном этапе они нам не нужны, возможно не понадобятся никогда. Просто рассматриваем `db.EntityName` как репозиторий и все.

Не буду тут много писать, мимокрокодил со стековерфлоу [все хорошо объяснил](https://stackoverflow.com/a/70646594)

## Простота лучше воровства

**Работаем по принципу "1 роут - 1 хендлер".**

Каждая функция, обслуживающая определенный роут, полостью самодостаточна (в плане DI и вообще) и в большинстве случаев будет предельно просто устроена:

1. В сигнатуре функции видно что она принимает и что она отдает.
2. Инжектимые зависимости это дтошки (1), ДБконтекст (2) и текущий юзер/профиль (3).
3. Тело - аккуратная цепочка EF квери, которая тупо в конце мапится на ДТОшку.

```cs
// TypedResult для openapi
public static async Task<Results<
    Ok<GetUserProfileOut>,
    NotFound<ErrorResponse>,
    InternalServerError<ErrorResponse>
  >>
  GetProfile(
    SoftwaresMarketDbContext db, // (2)
    Ctx ctx // (3)
  ) {
  var profile = await db.UserProfiles.Where(up => up.UserId == ctx.UserId)
    .Select(up => new GetUserProfileOut {
      UserId = up.UserId,
      ProfileId = up.Id,
      UserEmail = up.User.Email,
      ...
    })
    .FirstOrDefaultAsync();

  return profile is GetUserProfileOut result
    ? TypedResults.Ok(result)
    : TypedResults.NotFound(new ErrorResponse { Detail = "Profile not found." });
}
```

В `router.MapMethod("api/v1/some/endpoint", SomeHandler)` пишем полный путь.

По этому пути связанные с ним фронтовые квери ищутся через `ctrl + shift + F` на раз:

```ts
const someQuery = createQuery({
  effect: createApiEffect('get', 'api/v1/some/endpoint'),
});
```

### Допустимые в проекте композиции Minimal API

Все что внутри Market\<ModuleName\> - является статическим partial классом \<ModuleName\>:

```csharp
public static partial class <ModuleName> {}
```

Основная идея - дать возможность свободной композиции и реорганизации роутов и хендлеров в течении жизненного цилка проекта.

#### 1. Один файл Market/\<ModuleName\>/Endpoints.cs (1/2)

- Обработчики как лямбды в `MapGroup.MapMethod` или как статические методы в этом же файле.
- ДТОшки тут же

```csharp
namespace Market.<ModuleName>;

// DTO
public class SomeOut {}

public static partial class <ModuleName> {
    public static void Add<ModuleName>Endpoints(this WebApplication app) {

    var router = app.MapGroup("/")
      .RequireAuthorization()
      .WithOpenApi()
      .WithTags(["<ModuleName>"]);

    router.MapGet(
      "/api/v1/some",
      async Task<Results<Ok<SomeOut>, NotFound<ErrorResponse>>> (AppDbContext db, Ctx ctx) => {...});
    ...
  }

  // Или так
  public static async Task<Results<Ok<SomeOut>, NotFound<ErrorResponse>>> GetSome(AppDbContext db, Ctx ctx) {...}
}
```

#### 2. Вынос обработчиков в отдельные файлы Market/\<ModuleName\>/Handlers/\<HandlerName\>.cs (2/2)

- Поштучно, если обработчик очень толстый, например ProfileUpdate
- По группе соседних роутов, например Avatar (CRUD).
- ДТОшки лежат рядом со своими хендлерами (!)

```csharp
namespace Market.<ModuleName>;

public class SomeOut {}

public static partial class <ModuleName> {
  public static async Task<Results<Ok<SomeOut>, NotFound<ErrorResponse>>> GetSome(AppDbContext db, Ctx ctx) {...}
}
```

Более сложные варианты варианты, например такие как инжект хедлеров или репозиториев через DI, _пока не делаем_.

## Контекст

Весь общий код привязанный к запросу/текущему юзеру просто лежит в общей свалке Обертке вокруг HttpContext'а:

```cs
public class Ctx(HttpContext httpContext, SoftwaresMarketDbContext db) {
  public ClaimsPrincipal User => httpContext.User;
  ...

  public async Task<UserProfile?> CurrentProfile() {
    return await db.UserProfiles.Where(up => up.UserId == UserId).FirstOrDefaultAsync();
  }
}
```

Называется не Profile, а так, чтобы оставить место для scoped-свалки.
