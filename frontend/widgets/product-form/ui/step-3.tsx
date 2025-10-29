// import { useField } from '@tanstack/react-form';
// import { useUnit } from 'effector-react';
// import { Avatar } from 'primereact/avatar';
// import { Button } from 'primereact/button';
// import { Column } from 'primereact/column';
// import { DataTable } from 'primereact/datatable';
// import { Divider } from 'primereact/divider';
// import { Dropdown } from 'primereact/dropdown';
// import { InputTextarea } from 'primereact/inputtextarea';
// import { useState } from 'react';
// import { HeadingLevel, NameInput } from '~/shared/ui/components';
// import { Subscription, nextStep, prevStep, productForm } from '~/widgets/product-form/model.ts';
// import { useDataField } from '../product-form';

// const subscriptionTypes = [
//   { label: 'Фиксированный план', value: 'basic' },
//   { label: 'Пробный период', value: 'premium' },
//   { label: 'Индивидуальный план', value: 'enterprise' },
// ];

// const features = [
//   { label: 'Облачное хранилище', value: 'cloud' },
//   { label: 'Техподдержка 24/7', value: 'support' },
//   { label: 'Дополнительные интеграции', value: 'integrations' },
// ];

// export const Step3 = () => {
//   const { form } = useUnit(productForm);
//   const [prev, next] = useUnit([prevStep, nextStep]);

//   const subscriptionsField = useDataField('subscriptions');

//   const [currentSubscriptions, setCurrentSubscriptions] = useState<Subscription[]>([
//     {
//       name: '',
//       type: '',
//       description: '',
//       included: [],
//     },
//   ]);

//   const addSubscription = () => {
//     const newSubscription: Subscription = {
//       name: '',
//       type: '',
//       description: '',
//       included: [],
//     };
//     setCurrentSubscriptions((prev) => [...prev, newSubscription]);
//   };

//   const removeSubscription = (index: number) => {
//     const updatedSubscriptions = currentSubscriptions.filter((_, i) => i !== index);
//     setCurrentSubscriptions(updatedSubscriptions);
//   };

//   return (
//     <div>
//       <div className="flex flex-column gap-6">
//         {currentSubscriptions.map((subscription, index) => (
//           // biome-ignore lint/suspicious/noArrayIndexKey: <explanation>
//           <div key={index}>
//             <div className="flex gap-2 w-full justify-between">
//               <NameInput
//                 name={`subscription-name-${index}`}
//                 value={subscription.name}
//                 headingLevel={HeadingLevel.H2}
//                 onChange={(val) => {
//                   const updatedSubscriptions = [...currentSubscriptions];
//                   updatedSubscriptions[index].name = val;
//                   setCurrentSubscriptions(updatedSubscriptions);
//                 }}
//               />
//               <Avatar
//                 shape="circle"
//                 icon="icon icon-trash icon-2xl"
//                 pt={{
//                   root: {
//                     className: 'bg-transparent cursor-pointer',
//                   },
//                 }}
//                 onClick={() => removeSubscription(index)}
//               />
//             </div>
//             <Divider className="mt-3 mb-4" />

//             <div className="flex flex-column gap-3">
//               <section className="flex flex-column gap-1 relative">
//                 <label htmlFor={`type-${index}`} className="text-color-secondary text-sm">
//                   Тип подписки
//                 </label>
//                 <Dropdown
//                   name={`type-${index}`}
//                   value={subscription.type}
//                   options={subscriptionTypes}
//                   placeholder="Выберите тип подписки"
//                   onChange={(e) => {
//                     const updatedSubscriptions = [...currentSubscriptions];
//                     updatedSubscriptions[index].type = e.value;
//                     setCurrentSubscriptions(updatedSubscriptions);
//                   }}
//                   className="w-28rem"
//                 />
//               </section>

//               <section className="flex flex-column gap-1 relative">
//                 <label htmlFor={`language-${index}`} className="text-color-secondary text-sm">
//                   Описание
//                 </label>
//                 <InputTextarea
//                   name={`language-${index}`}
//                   value={subscription.description}
//                   rows={4}
//                   placeholder="Введите описание"
//                   onChange={(e) => {
//                     const updatedSubscriptions = [...currentSubscriptions];
//                     updatedSubscriptions[index].description = e.target.value;
//                     setCurrentSubscriptions(updatedSubscriptions);
//                   }}
//                   className="w-full"
//                 />
//               </section>

//               <section className="flex flex-column gap-1 relative">
//                 <DataTable
//                   value={features}
//                   selection={subscription.included}
//                   selectionMode="multiple"
//                   onSelectionChange={(e) => {
//                     const updatedSubscriptions = [...currentSubscriptions];
//                     updatedSubscriptions[index].included = e.value;
//                     setCurrentSubscriptions(updatedSubscriptions);
//                   }}
//                   dataKey="id"
//                   tableStyle={{ minWidth: '50rem' }}
//                 >
//                   <Column selectionMode="multiple" headerStyle={{ width: '3rem' }} />
//                   <Column field="label" header="Имя" />
//                 </DataTable>
//               </section>
//             </div>
//           </div>
//         ))}
//       </div>

//       <a className="no-underline text-indigo-400 cursor-pointer mt-6 block" onClick={addSubscription}>
//         + Добавить вариант подписки
//       </a>
//       <div className="flex justify-content-around align-items-center gap-8 px-5 mt-8">
//         <Button label="Назад" onClick={prev} size="large" outlined className="w-full" />
//         <Button label="Сохранить" onClick={next} size="large" className="w-full" />
//       </div>
//     </div>
//   );
// };
