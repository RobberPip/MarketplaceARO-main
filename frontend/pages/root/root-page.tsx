import { Layout } from '~/shared/ui/layout';
import { Header } from '~/widgets/header';
import './model';
import { Divider } from 'primereact/divider';
export function RootPage() {
  return (
    <Layout header={<Header />} mainClassName="p-3">
      <h1>Home</h1>
      <div className="p-component">
        <h2>Test Icons</h2>
        <div>
          tabler <span style={{ width: '2rem', height: '2rem', maskSize: '2rem' }} className="icon icon-number-wrong" />
        </div>
        <div>
          tabler <span className="icon icon-message" />
        </div>
        <div>
          prime <span className="icon icon-brand-android" />
        </div>
        <Divider />
      </div>
    </Layout>
  );
}
