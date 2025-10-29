# UI

### Мы используем

- https://primereact.org/ - Готовые React компоненты
- https://primeflex.org/ - сss-хелперы
- https://github.com/css-modules/css-modules

В качестве основы будем потихоньку пилить свою тему:

- https://primereact.org/theming/#customtheme
- [frontend/shared/assets/themes/aro](frontend/shared/assets/themes/aro)

### Что мы хотим получить

Cвою собственную библиотеку компонентов (поверх primereact).

Но так как как у нас нет ни ресурсов, ни компетенций чтобы сделать ее сразу, будем искать компромиссы в процессе.

### Как работаем

1. Внимательно изучаем библиотеку и стараемся брать готовые компоненты. Что можно сделать с компонентом и какие у него есть классы описано на вкладках API, Theming и PT
2. По ходу дела, в процессе работы:

   - переопределяем переменные в [теме aro](frontend/shared/assets/themes/aro)
   - там где не получается обойтись переменными, навешиваем доп. стили на классы primereact: [Card как пример](frontend/shared/assets/themes/aro/components/_card.scss)

3. Все что касается layout'а (компоненты контейнеры, отсупы) делаем с помощью классов primeflex

4. Не спешим выносить код в shared.
5. Начинаем всегда работать от страницы
6. Если есть готовый компонент - берем его
7. При переопределении свойств компонента, смотрим - это свойство дожно быть одинаковым везде, или привязанно только к конкретной странице/виджету? Если первое - лезем в тему, если второе - дорабатываем компонент по месту.

**Пример с хедером** ([Menubar](https://primereact.org/menubar/)) [9ce4ff3](https://git.arodevelop.ru/sandbox/dotnet-playground/-/merge_requests/2/diffs?commit_id=9ce4ff3d1cff07eb85965698c7992c5d2362884b)

Цвет фона - точно будет почти везде один, поэтому идем в frontend/shared/assets/themes/aro/variables/\_menu.scss
и меняем цвет фона на белый.

```scss
$horizontalMenuBg: #fff;
```

Привязка дочерних элементов внутри хедера - уже не такая основательная история, _возможно_ нам это не нужно везде.  
Поэтому добавляем привязку с помощью класса primeflex к рутовому элементу компонента в рамках конкретной страницы или виджета:

```tsx
// widgets/header
<Menubar
  start={'ARO'}
  model={[{ label: 'Главная' }, { label: 'Войти' }]}
  pt={{
    root: { className: 'justify-content-between' },
  }}
/>
```

## Иконки

В качестве основного набора иконок используется https://tabler.io/icons.

> [!WARNING]  
> Возможно это будет какой-то другой наборок иконок из этих:
>
> - https://iconoir.com/
> - https://lucide.dev/
> - https://remixicon.com/
> - https://icons.getbootstrap.com/
>
> Но это не принципиально.

C помощью [tools/icons.ts](../tools/icons.ts) при сборке они инлайнятся и сохраняются в [frontend/shared/assets/icons.css](../frontend/shared/assets/icons.css).

В [icons.css](./frontend/shared/assets/icons.css) попадают только иконки перечисленные в tablerIcons:

```ts
const tablerIcons = [
  // ADD NEW ICON HERE
  'cube',
  'user-circle',
  'shopping-cart',
  ...
]
```

И все иконки которые лежат в [frontend/shared/assets/icons](../frontend/shared/assets/icons).

Используются так же как обычные шрифтовые иконки:

```tsx
<span className="icon icon-cube" />     // 1.5rem
<span className="icon-2xl icon-cube" /> // 1.5rem
<span className="icon-4xl icon-cube" /> // 2rem

// за цвет отвечает background-color
<span className="icon-4xl icon-aro-logo bg-gray-900" />
```
