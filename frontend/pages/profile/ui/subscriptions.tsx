import { Divider } from 'primereact/divider';

export function SubscriptionsView() {
  return (
    <>
      <h1 id="subscriptions-h">Подписки</h1>
      <h2 id="active-subscriptions-h">Активные подписки</h2>
      <Divider />
      <div style={{ height: '500px' }} />
      <h2 id="inactive-subscriptions-h">Неактивные подписки</h2>
      <Divider />
      <div style={{ height: '500px' }} />
      <h2 id="wishlist-h">Список желаемого</h2>
      <Divider />
      <div style={{ height: '500px' }} />
    </>
  );
}
