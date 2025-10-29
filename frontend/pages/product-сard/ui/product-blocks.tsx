import { useUnit } from 'effector-react';
import { Button } from 'primereact/button';
import { Card, CardPassThroughOptions } from 'primereact/card';
import { Divider } from 'primereact/divider';
import { Galleria } from 'primereact/galleria';
import { Image } from 'primereact/image';
import { Inplace, InplaceContent, InplaceDisplay } from 'primereact/inplace';
import { Rating } from 'primereact/rating';
import { useState } from 'react';
import testImage from '~/shared/assets/test.jpg';
import { productQuery } from '../model.ts';
import { mapAttributes } from '~/shared/untils/mapAttributes.ts';
import { ProductAttributeName } from '~/shared/types/productAttributes.ts';

export const SelectedBlock = () => {
  // позволяет динамически вычислять какой блок вставлять в сетку
  const [data] = useUnit([productQuery.$data]);
  if (!data) {
    return null;
  }
  if (data.name == null) {
    return null;
  }

  return <CommandsBlock />;
};
export const SpecificationsBlock = () => {
  const data = useUnit(productQuery.$data.map((q) => q?.productData));
  const productAttributesMap = mapAttributes<ProductAttributeName>(data ?? []);
  return (
    <Card
      className="flex flex-column gap-4"
      title={<h6 className="flex flex-nowrap justify-content-between text-xl mb-0 mt-0">Характеристики</h6>}
      pt={
        {
          title: { className: 'm-0' },
          root: { className: 'p-3' },
          content: { className: 'flex flex-column gap-3 pb-0' },
        } satisfies CardPassThroughOptions
      }
    >
      <Divider />
      <div className="flex flex-column gap-5">
        <div>
          <h4 className="font-semibold mt-0 mb-3">Общая информация</h4>
          <ul className="list-none p-0 m-0 font-normal text-sm flex flex-column gap-2">
            <li className="flex flex-column sm:flex-row">
              <span className="sm:w-14rem text-color-secondary">Применение</span>
              <span>{productAttributesMap.Activity ?? '-'}</span>
            </li>
            <li className="flex flex-column sm:flex-row">
              <span className="sm:w-14rem text-color-secondary">Тип работы</span>
              <span>{productAttributesMap.WorkType ?? '-'}</span>
            </li>
          </ul>
        </div>
        <div>
          <h4 className="font-semibold mt-0 mb-3">Классификация</h4>
          <ul className="list-none p-0 m-0 font-normal text-sm flex flex-column gap-2">
            <li className="flex flex-column sm:flex-row">
              <span className="sm:w-14rem text-color-secondary">Стадия жизненного цикла</span>
              <span>{productAttributesMap.StageType ?? '-'}</span>
            </li>
            <li className="flex flex-column sm:flex-row">
              <span className="sm:w-14rem text-color-secondary">Бизнес-процесс</span>
              <span className="text-400">В работе</span>
            </li>
            <li className="flex flex-column sm:flex-row">
              <span className="sm:w-14rem text-color-secondary">Класс</span>
              <span>{productAttributesMap.ClassificationContent ?? '-'}</span>
            </li>
            <li className="flex flex-column sm:flex-row">
              <span className="sm:w-14rem text-color-secondary">Объект воздействия</span>
              <span>Продукт</span>
            </li>
            <li className="flex flex-column sm:flex-row">
              <span className="sm:w-14rem text-color-secondary">Марка проекта</span>
              <span>{productAttributesMap.ProjectMark ?? '-'}</span>
            </li>
          </ul>
        </div>
        <div>
          <h4 className="font-semibold mt-0 mb-3">Совеместимость</h4>
          <ul className="list-none p-0 m-0 font-normal text-sm flex flex-column gap-2">
            <li className="flex flex-column sm:flex-row">
              <span className="sm:w-14rem text-color-secondary">Основной продукт</span>
              <span>Основное/профильное</span>
            </li>
            <li className="flex flex-column sm:flex-row">
              <span className="sm:w-14rem text-color-secondary">Работает с ЦИМ</span>
              <span>{productAttributesMap.WorkCIM ? 'Да' : 'Нет'}</span>
            </li>
            <li className="flex flex-column sm:flex-row">
              <span className="sm:w-14rem text-color-secondary">Зарубежные аналоги</span>
              <span className="text-400">В работе</span>
            </li>
          </ul>
        </div>
        <div>
          <h4 className="font-semibold mt-0 mb-3">Публичные списки</h4>
          <ul className="list-none p-0 m-0 font-normal text-sm flex flex-column gap-2">
            <li className="flex flex-column sm:flex-row">
              <span className="sm:w-14rem text-color-secondary">Вендор состоит в НОТИМ</span>
              <span>Основное/профильное</span>
            </li>
            <li className="flex flex-column sm:flex-row">
              <span className="sm:w-14rem text-color-secondary">В реестре отечественного ПО</span>
              <span>Продукт</span>
            </li>
            <li className="flex flex-column sm:flex-row">
              <span className="sm:w-14rem text-color-secondary">
                Код записи в реестре
                <br /> отечественного ПО
              </span>
              <span>В работе</span>
            </li>
            <li className="flex flex-column sm:flex-row">
              <span className="sm:w-14rem text-color-secondary">
                Имя в реестре
                <br /> отечественного ПО
              </span>
              <span>В работе</span>
            </li>
          </ul>
        </div>
        <div>
          <h4 className="font-semibold mt-0 mb-3">Технические характеристики</h4>
          <ul className="list-none p-0 m-0 font-normal text-sm flex flex-column gap-2">
            <li className="flex flex-column sm:flex-row">
              <span className="sm:w-14rem text-color-secondary">ОС</span>
              <span>Основное/профильное</span>
            </li>
            <li className="flex flex-column sm:flex-row">
              <span className="sm:w-14rem text-color-secondary">Язык</span>
              <span>Продукт</span>
            </li>
            <li className="flex flex-column sm:flex-row">
              <span className="sm:w-14rem text-color-secondary">Размер</span>

              <span>В работе</span>
            </li>
            <li className="flex flex-column sm:flex-row">
              <span className="sm:w-14rem text-color-secondary">Дата выпуска</span>
              <span>В работе</span>
            </li>
            <li className="flex flex-column sm:flex-row">
              <span className="sm:w-14rem text-color-secondary">Дата изменения</span>
              <span>В работе</span>
            </li>
            <li className="flex flex-column sm:flex-row">
              <span className="sm:w-14rem text-color-secondary">Информация о версии</span>
              <span>В работе</span>
            </li>
            <li className="flex flex-column sm:flex-row">
              <span className="sm:w-14rem text-color-secondary">Поддержка клиентов</span>
              <span>В работе</span>
            </li>
            <li className="flex flex-column sm:flex-row">
              <span className="sm:w-14rem text-color-secondary">Совместимость</span>
              <span>В работе</span>
            </li>
          </ul>
        </div>
      </div>
    </Card>
  );
};
export const DescriptionBlock = () => {
  // show more
  const [active, onToggle] = useState(false);
  const [data] = useUnit([productQuery.$data]);

  if (!data) {
    return null;
  }
  return (
    <Card
      className="flex flex-column"
      title={
        <h3 className="flex flex-nowrap justify-content-between mb-2 mt-0">
          <div className="max-w-29rem text-4xl" style={{ wordWrap: 'break-word' }}>
            {data.name}
          </div>
          <span className="mt-1">
            <span className="icon-4xl icon-bookmark mr-4 text-4xl" />
            <span className="icon-4xl icon-share text-4xl" />
          </span>
        </h3>
      }
      footer={
        <span>
          <h4 className="font-semibold mt-0 mb-3">Описание продукта</h4>
          <span className="font-normal text-sm">{data.description}</span>
        </span>
      }
      pt={
        {
          root: { className: 'p-3' },
          content: { className: 'flex flex-column gap-3' },
        } satisfies CardPassThroughOptions
      }
    >
      <div className="flex gap-3">
        {/* <Rating value={data.rating} readOnly cancel={false} /> */}
        {/* <h4 className="font-semibold m-0">{data.rating.toFixed(1)}</h4> */}
      </div>
      <div className="block">
        <a href={data.website ?? undefined} className="block underline my-0 text-black text-primary ">
          Ссылка на первоисточник ↗
        </a>
        <p className="m-0 mt-3">
          Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et
        </p>
        <Inplace className="mt-1" unstyled onToggle={({ value }) => onToggle(value)} active={active}>
          <InplaceDisplay>
            <Button link className="p-0" style={{ color: '#6E86EF' }}>
              Показать больше
            </Button>
          </InplaceDisplay>
          <InplaceContent>
            <p className="m-0">Duis aute irure dolor in reprehenderit in voluptate velit</p>
            <Button onClick={() => onToggle(false)} link className="p-0" style={{ color: '#6E86EF' }}>
              Скрыть
            </Button>
          </InplaceContent>
        </Inplace>
        <Button className="mt-5 h-3rem sm:w-16rem font-semibold" label="Выбрать план подписки" rounded />
      </div>
    </Card>
  );
};
export const CommandsBlock = () => {
  const maxItemsToShow = 4;
  const [showMoreState, setShowMoreState] = useState<boolean[]>([]);
  const [showAll, setShowAll] = useState<boolean>(false);
  const toggleText = (index: number) => {
    // Переключаем состояние для конкретного элемента
    setShowMoreState((prevState) => {
      const newState = [...prevState];
      newState[index] = !newState[index];
      return newState;
    });
  };
  // TODO Изменить модель данных
  const commands = [
    { id: 1, name: 'Имя команды 1', version: '1.0.0', description: 'Текст описания команды' },
    { id: 2, name: 'Имя команды 2', version: '1.0.0', description: 'Текст описания команды' },
    { id: 3, name: 'Имя команды 3', version: '1.0.0', description: 'Текст описания команды' },
    { id: 4, name: 'Имя команды 4', version: '1.0.0', description: 'Текст описания команды' },
    { id: 5, name: 'Имя команды 5', version: '1.0.0', description: 'Текст описания команды' },
    { id: 6, name: 'Имя команды 6', version: '1.0.0', description: 'Текст описания команды' },
  ];

  return (
    <Card
      className="flex flex-column"
      title={
        <h6 className="flex flex-nowrap justify-content-between text-xl mb-0 mt-0">Список команд в составе плагина</h6>
      }
      footer={
        <Button link className="p-0" style={{ color: '#6E86EF' }} onClick={() => setShowAll((prev) => !prev)}>
          {showAll ? 'Скрыть' : 'Показать все'}
        </Button>
      }
      pt={
        {
          title: { className: 'm-0' },
          root: { className: 'p-3' },
          content: { className: 'flex flex-column gap-4 pb-0' },
        } satisfies CardPassThroughOptions
      }
    >
      <Divider />
      <ul className="list-none p-0 m-0">
        {commands.slice(0, showAll ? commands.length : maxItemsToShow).map((team) => (
          <li key={team.id} className="flex items-start border-bottom-1 border-300">
            <img
              alt="Иконка"
              className="border-circle border-1 mt-3 mb-4"
              style={{ width: '3.5rem', height: '3.5rem' }}
            />
            <div className="flex flex-column ml-4 gap-2">
              <h4 className="mt-3 font-semibold m-0">{team.name}</h4>
              <span className="text-color-secondary mb-3">
                {team.version} | {team.description}
                {showMoreState[team.id] && (
                  <span>
                    <br />
                    Описание команды
                  </span>
                )}
                <Button link className="p-0 ml-2" style={{ color: '#6E86EF' }} onClick={() => toggleText(team.id)}>
                  {showMoreState[team.id] ? 'Скрыть' : 'Показать больше'}
                </Button>
              </span>
            </div>
          </li>
        ))}
      </ul>
    </Card>
  );
};

// TODO: tmp
const images = [
  {
    itemImageSrc: testImage,
    thumbnailImageSrc: testImage, // Миниатюрное изображение
    alt: 'Image 2',
    title: 'Title 2',
  },
  {
    itemImageSrc: testImage,
    thumbnailImageSrc: testImage, // Миниатюрное изображение
    alt: 'Image 2',
    title: 'Title 2',
  },
  {
    itemImageSrc: testImage,
    thumbnailImageSrc: testImage, // Миниатюрное изображение
    alt: 'Image 2',
    title: 'Title 2',
  },
  {
    itemImageSrc: testImage,
    thumbnailImageSrc: testImage, // Миниатюрное изображение
    alt: 'Image 2',
    title: 'Title 2',
  },
  {
    itemImageSrc: testImage,
    thumbnailImageSrc: testImage, // Миниатюрное изображение
    alt: 'Image 2',
    title: 'Title 2',
  },
  {
    itemImageSrc: testImage,
    thumbnailImageSrc: testImage, // Миниатюрное изображение
    alt: 'Image 2',
    title: 'Title 2',
  },
  {
    itemImageSrc: testImage,
    thumbnailImageSrc: testImage, // Миниатюрное изображение
    alt: 'Image 2',
    title: 'Title 2',
  },
  {
    itemImageSrc: testImage,
    thumbnailImageSrc: testImage, // Миниатюрное изображение
    alt: 'Image 2',
    title: 'Title 2',
  },

  // добавьте больше изображений
];
type ImageItem = (typeof images)[number];

export const MediaCard = () => {
  return (
    <Card
      className="flex flex-column"
      title={<h6 className="flex flex-nowrap justify-content-between text-xl mb-0 mt-0">Скриншоты и видео</h6>}
      pt={{
        title: { className: 'm-0' },
        root: { className: 'p-3' },
        content: { className: 'flex flex-column gap-4 pb-0' },
      }}
    >
      <Divider />
      <div className="flex justify-content-center align-items-center">
        <Galleria
          className="flex-grow-1 w-10rem"
          value={images}
          responsiveOptions={[
            { breakpoint: '1250px', numVisible: 4 },
            { breakpoint: '1024px', numVisible: 3 },
            { breakpoint: '900px', numVisible: 2 },
          ]}
          numVisible={5}
          circular
          item={(item: ImageItem) => (
            <Image
              src={item.itemImageSrc}
              alt={item.alt}
              className="w-full"
              width="100%"
              preview
              pt={{
                root: { className: 'flex col-12 p-0 z-0' },
              }}
            />
          )}
          thumbnail={(item: ImageItem) => (
            <img
              src={item.thumbnailImageSrc}
              alt={item.alt}
              style={{ width: '80px' }} // Уменьшаем размер миниатюры
            />
          )}
        />
      </div>
    </Card>
  );
};

export const ReviewsBlock = () => {
  const maxItemsToShow = 3;
  const [showAll, setShowAll] = useState<boolean>(false);
  const reviews = [
    { id: 1, name: 'Сергей Л.', date: '24.11.2023', description: 'Текст отзыва', rating: 2 },
    { id: 2, name: 'Сергей Л.', date: '24.11.2023', description: 'Текст отзыва', rating: 3 },
    { id: 3, name: 'Сергей Л.', date: '24.11.2023', description: 'Текст отзыва', rating: 4 },
    { id: 4, name: 'Сергей Л.', date: '24.11.2023', description: 'Текст отзыва', rating: 1 },
    { id: 5, name: 'Сергей Л.', date: '24.11.2023', description: 'Текст отзыва', rating: 2 },
  ];
  return (
    <Card
      className="flex flex-column"
      title={
        <h6 className="flex flex-nowrap justify-content-between mb-0 mt-0">
          <div className="text-xl">
            Отзывы ({reviews.length}) {/*TODO Изменить на данные*/}
          </div>
          <span className="icon icon-filter text-2xl mt-1" />
        </h6>
      }
      footer={
        <Button link className="p-0" style={{ color: '#6E86EF' }} onClick={() => setShowAll((prev) => !prev)}>
          {showAll ? 'Скрыть' : 'Показать все'}
        </Button>
      }
      pt={{
        title: { className: 'm-0' },
        root: { className: 'p-3' },
        content: { className: 'flex flex-column gap-4 pb-0' },
      }}
    >
      <Divider />
      <div className="flex justify-content-between">
        <div className="flex flex-column">
          <h4 className="font-semibold mt-0 mb-2">Ваш отзыв</h4>
          <span className="text-sm text-color-secondary">Оставьте свой отзыв об использовании продукта</span>
        </div>
        <Button
          label="Написать отзыв"
          className="font-semibold bg-white justify-content-center text-primary border-2"
          rounded
        />
      </div>
      <ul className="list-none p-0 m-0 flex flex-column gap-3">
        {reviews.slice(0, showAll ? reviews.length : maxItemsToShow).map((review) => (
          <li key={review.id} className=" border-bottom-1 border-300">
            <div className="flex">
              <img
                alt="Иконка"
                className="mr-3 mb-3 border-circle border-1"
                style={{ width: '43px', height: '43px' }}
              />
              <div className="flex w-full flex-column">
                <div className="flex justify-content-between">
                  <h4 className="font-semibold mt-0 mb-0">{review.name}</h4>
                  <div className="flex gap-3">
                    <Rating value={review.rating} readOnly cancel={false} />
                    <h4 className="font-semibold m-0">{review.rating.toFixed(1)}</h4>
                  </div>
                </div>
                <span className="text-xs text-color-secondary mt-1">{review.date}</span>
              </div>
            </div>
            <div className="mb-3">
              <span className="text-sm text-color-secondary">{review.description}</span>
            </div>
          </li>
        ))}
      </ul>
    </Card>
  );
};
