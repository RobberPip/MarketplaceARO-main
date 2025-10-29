# Data Fetching

Как мы загружаем и обрабатываем данные на нашем фронте.

## Motivation

Работа с данными на фронтенде концептуально весьма сложная штука.

![Frontend-Meme-1](./images/frontend-meme-1.png)

Мемасы это конечно хорошо. Но все таки, почему мы просто не можем делать как-то так?:

```js
fetch('/api/data')
  .then((response) => response.json())
  .then((data) => {
    // Just insert data into DOM
  });
```

Потому что даже на небольшом приложении, это все очень быстро выходит из под контроля.  
Чем больше бизнес логики в нашем приложении, чем больше эжд-кейсов мы учитываем, тем сильнее и быстрее кодовая база превращается в решето.

Список того что нам нужно или может понадобиться при загрузке данных:

- Обработка ошибок: сетевых, валидации и ошибки самой бизнес логики
- ретраи
- проверка и актуализация данных в фоне
- синхронизация загруженных данных в различных частях приложения друг с другом и с бэкендом
- кеширование
- дедупликация запросов
- и много другое.

Подробно проблематика описана в документации [React Query](https://tanstack.com/query/latest/docs/framework/solid/overview#motivation).  
Но если кратко, нам не просто нужен клиент, который выполняет http-запросы, нам нужен целый комбайн вокруг этого клиента, который возьмет на себя все перечисленные выше проблемы\*.

> [!NOTE]  
> \*Которых, понятное дело, не было бы, не тащи мы логику на фронтенд.  
> SSR, HTMX, бла-бла-бла.

В нашем случае таким инструментом является [Farfecthed](https://ff.effector.dev/tutorial/).

## Farfecthed

- https://ff.effector.dev/tutorial/
- https://ff.effector.dev/recipes/
- https://ff.effector.dev/api/

Все что нужно знать о Farfecthed есть по этим ссылкам.

## Наша специфика

В качестве "низкоуровневого" клиента мы используем [openapi-fetch](https://openapi-ts.dev/openapi-fetch/).  
Это небольшая обертка над нативным fetch'ем, которая дает нам полную статическую типизацию для нашего API.

> [!NOTE]  
> Полнота типизации напрямую зависит от полноты и корректности OpenAPI спеки с бекенда.

```ts
import createFetchClient from 'openapi-fetch';
import { createClient } from 'openapi-ff';
import { createQuery } from '@farfetched/core';
import type { paths } from './schema'; // схема сгенерированная с помощью openapi-typescript

// статически типизированный openapi-fetch клиент
// им можно пользоваться напрямую, но мы заворочиваем его в эффекты ниже
export const client = createFetchClient<paths>({
  baseUrl: 'https://myapi.dev/v1/',
});

// фабрика типизированных эффектов
export const { createApiEffect } = createClient(client);

// сама квери
const blogpostQuery = createQuery({
  effect: createApiEffect('get', '/blogposts/{post_id}'),
});
```

## Что делает query

Квери отвечает как за загрузку так и за хранение данных. Можно обозвать ее Per Route Singleton Manager 😁.  
Некая сущность, отвечающая за запросы на один единственный эндпоинт, и хранящая в себе результат выполнения этого запроса.

Так же как и эффект, квери является совокупностью нескольких юнитов, только сложнее.

```ts
const blogpostsQuery = createQuery({
  effect: createApiEffect('get', '/blogposts'),
});

// можно запустить или сбросить (.reset())
blogpostsQuery.start();

// события запроса
blogpostsQuery.finished.success;
blogpostsQuery.finished.failure;
blogpostsQuery.finished.finally; // success, failure or skip

// состояние квери
blogpostsQuery.$data; // результат успешного запроса: null |response
blogpostsQuery.$error; // null | error
blogpostsQuery.$status; // initial" | "pending" | "done" | "fail"
```

Полное АПИ тут:

- https://ff.effector.dev/api/primitives/query.html
- https://ff.effector.dev/api/primitives/mutation.html

### Что умеет query

Будет пополнятся по ходу.

Пока краткий перечень того что умеет квери:

- Можно связывать квери друг с другом. То есть автоматически обновлять (запускать) квери после успешного выполнения связанных https://ff.effector.dev/api/operators/connect_query.html

- Можно выставлять условия запуска/не запуска квери. что-то вроде `sample.filter`, но с более сложным и богатым АПИ https://ff.effector.dev/api/operators/apply_barrier.html

- Можно связывать с роутом. Например открывать роут только после успешного выполнения квери https://ff.effector.dev/api/routers/atomic-router.html

- Можно навешивать на квери операторы retry, timeout, cache и тд https://ff.effector.dev/api/operators/retry.html

### Как мы работаем с query

Мы работаем по методолгии FSD, в ее минимальной комплектации: app, pages, shared.

Соответственно ключевыми объектами у нас являются `<page_name>/models.ts`.

В сердце модели страницы - ее роут и N-oe количество кверей и мутаций.

> [!NOTE]  
> Про роуты и их взаимоотношения с квери и страницами пока опустим, сосредоточимся на квери.

Существующее бэкенд API мы можем подсмотреть так:

- просто перебирая аргументы в `createApiEffect('get', 'api/search/endopint/here')`
- в [shared/api/schema.d.ts](../frontend/shared/api/schema.d.ts). Тип `path` полностью соответствует исходной спеке. Выбрав нужный роут, можем перейти к его респонзу, а от туда к схемам.
- и само собой на [страничке сваггера](http://localhost:5000/swagger/v1/swagger.json)

По умолчанию, тип `query.$data` будет соответствовать nullable типу response в схеме:

```ts
// todos/model.ts
export const todosQuery = createQuery({
  effect: createApiEffect('get', '/api/v1/todos'),
});

// typeof data = null | components['schemas']['FindTodosOut'];
debug(todosQuery.$data); // null
```

Если мы хотим провести какую-то трансформацию перед сохранением или сделать какое-то дефолтное значение вместо null, то делаем так:

```ts
export const todosQuery = createQuery({
  effect: createApiEffect('get', '/api/v1/todos'),
  mapData: ({ result: { results } }) => results,
  initialData: [],
});

// typeof data = components['schemas']['Todo'][];
debug(todosQuery.$data); // [];
```

_У mapData есть более сложная форма [`mapData: { source, fn }`](https://ff.effector.dev/api/factories/create_query.html#createquery-effect-contract-validate-mapdata-source-fn-initialdata)_

Опционально можем навесить операторы:

```ts
export const todosQuery = createQuery({...});

concurrency(todosQuery, { strategy: 'TAKE_LATEST' });
retry(todosQuery, { times: 5, delay: exponentialDelay(1000)});
```

Если мы хотим, чтобы квери нельзя было выполнить по какому-то условию, используем оператор [`applyBarrier`](https://ff.effector.dev/recipes/barrier_circuit_breaker.html).

У нас есть готовая абстракция для `applyBarrier(buggyQuery, { barrier: authBarrier });` -
`applyAuthBarrier`:

```ts
applyAuthBarrier([todosQuery, addTodoMutation, updateTodoMutation, deleteTodoMutation]);
```
