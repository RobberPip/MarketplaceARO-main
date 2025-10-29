import { useList, useUnit } from 'effector-react';
import { Avatar } from 'primereact/avatar';
import { Button } from 'primereact/button';
import { Card, CardPassThroughOptions } from 'primereact/card';
import { Checkbox } from 'primereact/checkbox';
import { Chip } from 'primereact/chip';
import { Tag } from 'primereact/tag';
import React from 'react';
import { $checkedAgreements, updateCheckedAgreements } from '~/pages/profile/model.ts';
import { components } from '~/shared/api/schema';
import { routes } from '~/shared/routing.ts';
import { $productsList } from '../profile-products/model.ts';

export function SubscriptionsView() {
  const products = useList($productsList, {
    fn: (product) => <ProductCard product={product} />,
    placeholder: <EmptyProducts />,
  });

  return (
    <>
      <article className="flex flex-column mb-8">
        <div className="flex justify-content-between mb-4">
          <h1 className="m-0 text-4xl">Подписки</h1>
          <Button label="Добавить подписку" className="w-12rem w-full" />
        </div>
        {products}
      </article>
    </>
  );
}

const EmptyProducts = () => {
  const [checkedAgreements, setCheckedAgreements] = useUnit([$checkedAgreements, updateCheckedAgreements]);

  return (
    <form className="mt-8 flex flex-column align-items-center text-center">
      <div className="w-max">
        <h2 className="mt-7 mb-4 text-2xl font-medium">Нет активных продуктов</h2>
        <p className="text-500 mb-5 line-height-3">
          Добавьте продукты в реестр Arogroup, заполните необходимую
          <br />
          информацию, и помогайте людям находить решения для своих задач
        </p>
        <div className="flex">
          <Checkbox className="mr-3" onChange={(e) => setCheckedAgreements(!!e.checked)} checked={checkedAgreements} />
          <span className="text-500 text-left">
            Я подтверждаю, что ознакомлен и согласен с{' '}
            <a href="#" className="text-500">
              Соглашением <br />о распространении программных продуктов
            </a>
          </span>
        </div>
        <Button label="Добавить продукт" className="sm:w-23rem w-full mt-5 h-3rem" disabled={!checkedAgreements} />
      </div>
    </form>
  );
};

const ProductCard: React.FC<{
  product: components['schemas']['GetSoftwareOut'];
}> = ({ product }) => {
  return (
    <Card
      pt={
        {
          root: { className: 'mb-2' },
          content: { className: 'py-1 p-0 ' },
        } satisfies CardPassThroughOptions
      }
    >
      <section className="flex justify-content-between">
        <div className="flex">
          <Avatar
            image="#"
            shape="circle"
            className="mx-2 border-circle border-1"
            style={{ width: '56px', height: '56px' }}
          />

          <div className="ml-3 flex flex-column">
            <h2 className="text-base m-0 mb-2">{product.name}</h2>
            <span className="text-500 flex align-items-center">
              <Tag
                severity="success"
                className="border-circle p-0 mr-2"
                style={{
                  width: '6px',
                  height: '6px',
                }}
              />
              Подтверждено | 1.0.0 | 29.01.2024 | <Chip className="border-round-sm px-2 mx-2" label="Revit" />
            </span>
          </div>
        </div>
        <div className="text-500 flex ">
          <span className="text-500 flex align-items-center mr-3 font-semibold">143000 ₽</span>
          <span className="icon-2xl icon-dots-vertical mt-3 mr-2" />
        </div>
      </section>
    </Card>
  );
};
