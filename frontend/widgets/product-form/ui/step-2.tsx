// import { useField } from '@tanstack/react-form';
// import { useUnit } from 'effector-react';
// import { Button } from 'primereact/button';
// import { Calendar } from 'primereact/calendar';
// import { Divider } from 'primereact/divider';
// import { InputText } from 'primereact/inputtext';
// import { MultiSelect } from 'primereact/multiselect';
// import MediaPicker from '~/shared/ui/components/MediaPicker.tsx';
// import { useDataField } from '..';
// import { nextStep, prevStep } from '../model';

// export const Step2 = () => {
//   const [prev, next] = useUnit([prevStep, nextStep]);

//   const mediaField = useDataField('media');

//   const languageField = useDataField('language');

//   const osField = useDataField('os');

//   const releaseDateField = useDataField('releaseDate');

//   const updateDateField = useDataField('updateDate');

//   const versionInfoField = useDataField('versionInfo');

//   const customerSupportField = useDataField('customerSupport');

//   const applicationTypeField = useDataField('applicationType');

//   return (
//     <div className="flex flex-column gap-5">
//       <div className="flex flex-column">
//         <h2 className="m-0 text-xl font-medium">Фото/видео</h2>
//         <Divider className="mt-3 mb-4" />

//         <div className="flex flex-column gap-3">
//           <section className="flex flex-column gap-3 text-sm">
//             <div className="w-full text-color-secondary">
//               Нажмите на кнопку или перетащите в эту область до 10 изображений в формате JPEG или PNG или до 3-х видео
//               общей продолжительностью до 10 минут
//             </div>

//             <MediaPicker
//               multiple
//               accept="image/*,video/*"
//               onChange={(val) => {
//                 mediaField.handleChange(val);
//               }}
//             />
//           </section>
//         </div>
//       </div>

//       <div className="flex flex-column">
//         <h2 className="m-0 text-xl font-medium">Дополнительные данные</h2>
//         <Divider className="mt-3 mb-4" />

//         <div className="flex flex-column gap-3">
//           <section className="flex justify-content-between">
//             <div className="flex flex-column">
//               <h3 className="m-0 font-semibold text-base">
//                 Файл {applicationTypeField.state.value === 'General' ? 'продукта' : 'плагина'}
//               </h3>
//               <div className="text-color-secondary text-sm mt-2">Описание, характеристики файла</div>
//             </div>

//             <Button type="button" size="large" outlined label="Добавить файл" />
//           </section>
//         </div>
//       </div>

//       <div className="flex flex-column">
//         <h2 className="m-0 text-xl font-medium">Технические характеристики</h2>
//         <Divider className="mt-3 mb-4" />

//         <div className="flex flex-column gap-3">
//           <section className="flex flex-column gap-1 relative">
//             <label htmlFor="language" className="text-color-secondary text-sm">
//               Язык
//             </label>
//             <MultiSelect
//               display="chip"
//               value={languageField.state.value}
//               options={['Русский', 'Английский', 'Китайский']}
//               onChange={(e) => languageField.handleChange(e.value)}
//               placeholder="Выберите языки"
//               className="w-28rem"
//             />
//           </section>

//           <section className="flex flex-column gap-1">
//             <label htmlFor="os" className="text-color-secondary text-sm">
//               Операционные системы
//             </label>
//             <MultiSelect
//               display="chip"
//               value={osField.state.value}
//               options={['Windows', 'MacOS', 'Linux', 'Android', 'iOS']}
//               onChange={(e) => osField.handleChange(e.value)}
//               placeholder="Выберите ОС"
//               className="w-28rem"
//             />
//           </section>

//           <section className="flex flex-column gap-1">
//             <label htmlFor="releaseDate" className="text-color-secondary text-sm">
//               Дата выпуска
//             </label>
//             <Calendar
//               placeholder="__.__.____"
//               value={releaseDateField.state.value}
//               onChange={(e) => releaseDateField.handleChange(e.value ?? null)}
//               showIcon
//               dateFormat="dd.mm.yy"
//               selectionMode="single"
//               className="w-9rem"
//             />
//           </section>

//           <section className="flex flex-column gap-1">
//             <label htmlFor="updateDate" className="text-color-secondary text-sm">
//               Дата изменения
//             </label>
//             <Calendar
//               placeholder="__.__.____"
//               value={updateDateField.state.value}
//               onChange={(e) => updateDateField.handleChange(e.value ?? null)}
//               showIcon
//               dateFormat="dd.mm.yy"
//               selectionMode="single"
//               className="w-9rem"
//             />
//           </section>

//           <section className="flex flex-column gap-1">
//             <label htmlFor="versionInfo" className="text-color-secondary text-sm">
//               Информация о версии
//             </label>
//             <InputText
//               value={versionInfoField.state.value}
//               onChange={(e) => versionInfoField.handleChange(e.target.value)}
//               placeholder="1.0.0"
//               className="w-28rem"
//             />
//           </section>

//           <section className="flex flex-column gap-1">
//             <label htmlFor="customerSupport" className="text-color-secondary text-sm">
//               Поддержка клиентов
//             </label>
//             <InputText
//               value={customerSupportField.state.value}
//               onChange={(e) => customerSupportField.handleChange(e.target.value)}
//               placeholder="a.ivanov@yandex.ru"
//               className="w-28rem"
//             />
//           </section>
//         </div>
//       </div>

//       <div className="flex justify-content-around align-items-center gap-8 px-5 mt-8">
//         <Button label="Назад" onClick={prev} size="large" outlined className="w-full" />
//         <Button label="Далее" onClick={next} size="large" className="w-full" />
//       </div>
//     </div>
//   );
// };
