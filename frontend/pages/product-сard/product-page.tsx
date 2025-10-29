import { Layout } from '~/shared/ui/layout';
import { DescriptionBlock, SelectedBlock, ReviewsBlock, SpecificationsBlock, MediaCard } from './ui';
import { Header } from '~/widgets/header';

export const ProductPage = () => {
  return (
    <Layout header={<Header />}>
      <SoftwareCard />
    </Layout>
  );
};
// Деление идёт на Grid с двумя колоннками (левый и правый), в которые добовляются блоки
export const SoftwareCard = () => {
  return (
    <div className="grid m-0 lg:pl-7 pl-5 lg:pr-7 pr-5 gap-5">
      {/* Левая колонка */}
      <div className="col grid m-0 p-0 gap-5 pt-5 flex-column">
        <div className="col p-0 flex-grow-0">
          <DescriptionBlock />
        </div>
        {/* Динамический блок */}
        {SelectedBlock() ? <div className="col flex-order-2 p-0 flex-grow-0">{SelectedBlock()}</div> : null}
        <div className="col flex-order-2 p-0 flex-grow-0">
          <ReviewsBlock />
        </div>
      </div>
      {/* Правая колонка */}
      <div className="col grid m-0 p-0 gap-5 sm:pt-5 pt-0 flex-column">
        <div className="col p-0 flex-grow-0">
          <SpecificationsBlock />
        </div>
        <div className="col p-0 flex-grow-0">
          <MediaCard />
        </div>
      </div>
    </div>
  );
};
