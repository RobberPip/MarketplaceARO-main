import { RouteInstance } from 'atomic-router';
import { createEffect, createEvent, createStore, sample } from 'effector';
import { debug } from 'patronum';
import { MenuItem } from 'primereact/menuitem';
import { routes } from '~/shared/routing';

export type NavMenuItem = MenuItem & { parent?: string; route: RouteInstance<any> };

export const navigate = createEvent<string>();

const menuItemsBase = [
  {
    id: 'personal-data',
    label: 'Согласие на обработку персональных данных',
    className: 'mt-5',
  },

  {
    id: 'terms',
    label: 'Пользовательское соглашение',
  },
  { id: 'preambula-terms', label: 'Преамбула', parent: 'terms' },
  { id: 'general-terms', label: 'Общие положения', parent: 'terms' },
  { id: 'singup-terms', label: 'Регистрация', parent: 'terms' },
  { id: 'personal-terms', label: 'Личный кабинет', parent: 'terms' },
  { id: 'data-terms', label: 'Данные пользователя', parent: 'terms' },
  { id: 'conditions-terms', label: 'Назначения и условия использования IT market', parent: 'terms' },
  { id: 'reviews-terms', label: 'Отзывы и рейтинг', parent: 'terms' },
  { id: 'deal-terms', label: 'Условия совершения сделок', parent: 'terms' },
  { id: 'responsibility-terms', label: 'Ответственность', parent: 'terms' },
  { id: 'agreement-terms', label: 'Порядок изменения Соглашения', parent: 'terms' },
  { id: 'other-terms', label: 'Прочие положения', parent: 'terms' },
  { id: 'contacts-terms', label: 'Контакты', parent: 'terms' },

  {
    id: 'privacy',
    label: 'Политика конфиденциальности',
  },
  { id: 'definitions-privacy', label: 'Термины и определения', parent: 'privacy' },
  { id: 'general-privacy', label: 'Общие условия', parent: 'privacy' },
  { id: 'purposes-privacy', label: 'Цели обработки персональных данных', parent: 'privacy' },
  { id: 'methods-privacy', label: 'Способы обработки персональных данных', parent: 'privacy' },
  { id: 'term-privacy', label: 'Срок обработки персональных данных', parent: 'privacy' },
  { id: 'safety-privacy', label: 'Обеспечение безопасности персональных данных', parent: 'privacy' },
  { id: 'access-privacy', label: 'Доступ к персональным данным', parent: 'privacy' },
  { id: 'services-privacy', label: 'Сервисы третьих лиц', parent: 'privacy' },
  { id: 'rights-privacy', label: 'Ваши права', parent: 'privacy' },
  { id: 'cookie-privacy', label: 'Информация о cookie', parent: 'privacy' },
  { id: 'politics-privacy', label: 'Изменение и дополнение Политики', parent: 'privacy' },
  { id: 'connection-privacy', label: 'Обратная связь', parent: 'privacy' },

  {
    id: 'user-agreement',
    label: 'Соглашение о распространении программных продуктов',
  },
  { id: 'preambula-user-agreement', label: 'Преамбула', parent: 'user-agreement' },
  { id: 'general-user-agreement', label: 'Общие положения', parent: 'user-agreement' },
  { id: 'singup-user-agreement', label: 'Регистрация и Личный кабинет', parent: 'user-agreement' },
  { id: 'object-user-agreement', label: 'Предмет Соглашения', parent: 'user-agreement' },
  { id: 'deal-user-agreement', label: 'Условия использования IT market', parent: 'user-agreement' },
  { id: 'guarantees-user-agreement', label: 'Гарантии и заверения Разработчика', parent: 'user-agreement' },
  { id: 'rights-user-agreement', label: 'Права и обязанности Разработчика', parent: 'user-agreement' },
  { id: 'payment-user-agreement', label: 'Вознаграждение и оплата', parent: 'user-agreement' },
  { id: 'license-user-agreement', label: 'Лицензия', parent: 'user-agreement' },
  { id: 'responsibility-user-agreement', label: 'Ответственность', parent: 'user-agreement' },
  { id: 'term-user-agreement', label: 'Срок и порядок расторжения Соглашения', parent: 'user-agreement' },
  { id: 'politics-user-agreement', label: 'Изменение и дополнение Политики', parent: 'user-agreement' },
  { id: 'agreement-user-agreement', label: 'Порядок изменения Соглашения', parent: 'user-agreement' },

  { id: 'terms-use', label: 'Условия использования' },
  { id: 'definitions-terms-use', label: 'Термины и определения', parent: 'terms-use' },
  { id: 'general-terms-use', label: 'Общие положения', parent: 'terms-use' },
  { id: 'users-terms-use', label: 'Пользователи', parent: 'terms-use' },
  { id: 'registration-terms-use', label: 'Регистрация и авторизация', parent: 'terms-use' },
  { id: 'account-terms-use', label: 'Личный кабинет', parent: 'terms-use' },
  { id: 'it-market-usage-terms-use', label: 'Использования IT market', parent: 'terms-use' },
  { id: 'user-ip-terms-use', label: 'Интелектуальная собственность Пользователей', parent: 'terms-use' },
  { id: 'software-placement-terms-use', label: 'Размещение Программных продуктов', parent: 'terms-use' },
  {
    id: 'interaction-deals-terms-use',
    label: 'Взаимодействие на IT market и порядок заключения сделок',
    parent: 'terms-use',
  },
  { id: 'copyright-claims-terms-use', label: 'Порядок рассмотрения заявлений правообладателей', parent: 'terms-use' },
  { id: 'liability-terms-use', label: 'Ответственность', parent: 'terms-use' },
  { id: 'terms-update-terms-use', label: 'Порядок изменения Условий использования', parent: 'terms-use' },
  { id: 'termination-terms-use', label: 'Прекращение использования IT market', parent: 'terms-use' },
  { id: 'governing-law-terms-use', label: 'Применимое право и разрешение споров', parent: 'terms-use' },
  { id: 'final-provisions-terms-use', label: 'Заключительные положения', parent: 'terms-use' },
  { id: 'contacts-terms-use', label: 'Контакты', parent: 'terms-use' },
];

const $menuDict = createStore<Record<string, NavMenuItem>>(initmenuDict());
export const $menuItems = $menuDict.map((dict) => Object.values(dict));
export const $activeItemsIds = createStore<string[]>([]);
// Navigation
{
  sample({
    clock: navigate,
    source: $menuDict,
    fn: (menuDict, id) => {
      const items = [id];
      const parent = menuDict[id].parent;
      if (parent) {
        items.push(parent);
      }
      return items;
    },
    target: $activeItemsIds,
  });
  sample({
    clock: navigate,
    source: $menuDict,
    fn: (dict, id) => {
      return dict[id];
    },
    target: createEffect((item: NavMenuItem) => {
      routes.docs.navigate({
        params: {},
        query: { name: item.parent ?? item.id },
      });

      setTimeout(() => {
        const el = document.getElementById(`${item.id!}-h`);
        if (el) {
          el.style.scrollMarginTop = '72px'; // Heught header
          el.scrollIntoView({ block: 'start', behavior: 'smooth' });
        }
      }, 10);
    }),
  });
}

function initmenuDict() {
  const menuDict: Record<string, NavMenuItem> = {};
  for (const item of menuItemsBase) {
    (menuDict as any)[item.id] = item;
  }
  return menuDict;
}
