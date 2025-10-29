import React, { useState } from 'react';
import { useStoreMap, useUnit } from 'effector-react';
import { $menuItems, $activeItemsIds, NavMenuItem, navigate } from './model.ts';
import clsx from 'clsx';
import { router } from '~/shared/routing.ts';

export const DocsNavbar = React.memo(() => {
  const items = useStoreMap($menuItems, (items) => groupByParent(items));

  return (
    <nav className="w-23rem h-full overflow-y-auto">
      <ul className="p-0 m-0 list-none">
        {Object.entries(items).map(([parent, children]) => (
          <NavbarGroup key={parent} parent={parent} childrenItems={children} />
        ))}
      </ul>
    </nav>
  );
});

function NavbarGroup({ parent, childrenItems }: { parent: string; childrenItems: NavMenuItem[] }) {
  const [collapsed, setCollapsed] = useState(true);
  const parentItem = childrenItems.find((item) => item.id === parent);
  const isActiveRoute = useUnit(router.$query);
  const isActive = useStoreMap($activeItemsIds, (ids) => ids.includes(parent));
  const hasChildren = childrenItems.some((item) => item.parent === parent);

  return (
    <li className="mb-1">
      <div
        className={clsx('p-menuitem-content flex items-center justify-between cursor-pointer', {
          'text-color-secondary': !isActive && !(parent === isActiveRoute.name),
        })}
        style={{ paddingTop: '7px', paddingBottom: '7px' }}
      >
        {hasChildren ? (
          <span onClick={() => setCollapsed((prev) => !prev)} className={clsx('ml-7', parentItem?.className)}>
            {collapsed ? (
              <span className="icon-2xl icon-chevron-down" />
            ) : (
              <span className="icon-2xl icon-chevron-up" />
            )}
          </span>
        ) : (
          <span className="ml-8 pl-1" />
        )}
        <a
          onClick={() => !parentItem?.disabled && navigate(parentItem?.id!)}
          className={clsx('p-menuitem-link pl-2 font-semibold', parentItem?.className)}
        >
          {parentItem?.label ?? parent}
        </a>
      </div>

      {!collapsed && hasChildren && (
        <div className="cursor-pointer ml-8 pl-3 mr-3">
          {childrenItems
            .filter((item) => item.parent === parent)
            .map((item) => (
              <NavbarItem key={item.id} item={item} />
            ))}
        </div>
      )}
    </li>
  );
}

function NavbarItem({ item }: { item: NavMenuItem }) {
  const isActive = useStoreMap($activeItemsIds, (ids) => ids.includes(item.id!));
  const isActiveRoute = useUnit(router.$query);

  return (
    <div
      className={clsx('p-menuitem-content py-1', {
        'text-color-secondary': !isActive && !(item.id === isActiveRoute.name),
      })}
    >
      {'parent' in item ? (
        <a aria-current={isActive} className="p-menuitem-link py-1">
          <span
            onClick={() => {
              if (!item.disabled) {
                navigate(item.id!);
              }
            }}
            className="text-sm"
          >
            {item.label}
          </span>
        </a>
      ) : (
        <a
          aria-current={isActive}
          className={clsx('p-menuitem-link', 'p-2', 'pl-7 font-semibold', item.className)}
          onClick={() => !item.disabled && navigate(item.id!)}
        >
          {item.label}
        </a>
      )}
    </div>
  );
}

function groupByParent(items: NavMenuItem[]) {
  const groups: Record<string, NavMenuItem[]> = {};

  // biome-ignore lint/complexity/noForEach: <explanation>
  items.forEach((item) => {
    const parent = ('parent' in item && item.parent ? item.parent : item.id) ?? 'default';
    if (!groups[parent]) {
      groups[parent] = [];
    }
    groups[parent].push(item);
  });

  return groups;
}
