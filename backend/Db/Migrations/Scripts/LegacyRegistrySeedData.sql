
-- Software table
insert into public.software (id, name, description, link_to, logo, date_to_add, rating, review_count, like_count) values (1, 'Renga', null, null, null, '2025-03-10 19:10:35.344284', 0.00, 0, 0);
insert into public.software (id, name, description, link_to, logo, date_to_add, rating, review_count, like_count) values (2, 'Pilot-BIM', null, null, null, '2025-03-10 19:10:35.344284', 0.00, 0, 0);
insert into public.software (id, name, description, link_to, logo, date_to_add, rating, review_count, like_count) values (3, 'Revit', null, null, null, '2025-03-10 19:10:35.344284', 0.00, 0, 0);
insert into public.software (id, name, description, link_to, logo, date_to_add, rating, review_count, like_count) values (4, 'КОМПАС', null, null, null, '2025-03-10 19:10:35.344284', 0.00, 0, 0);
insert into public.software (id, name, description, link_to, logo, date_to_add, rating, review_count, like_count) values (5, 'Rider', null, null, null, '2025-03-10 19:10:35.344284', 0.00, 0, 0);
insert into public.software (id, name, description, link_to, logo, date_to_add, rating, review_count, like_count) values (6, 'Datagrip', null, null, null, '2025-03-10 19:10:35.344284', 0.00, 0, 0);
insert into public.software (id, name, description, link_to, logo, date_to_add, rating, review_count, like_count) values (7, 'VScode', null, null, null, '2025-03-10 19:10:35.344284', 0.00, 0, 0);
insert into public.software (id, name, description, link_to, logo, date_to_add, rating, review_count, like_count) values (8, 'Autocad', null, null, null, '2025-03-10 19:10:35.344284', 0.00, 0, 0);
insert into public.software (id, name, description, link_to, logo, date_to_add, rating, review_count, like_count) values (9, 'Newisworks', null, null, null, '2025-03-10 19:10:35.344284', 0.00, 0, 0);
insert into public.software (id, name, description, link_to, logo, date_to_add, rating, review_count, like_count) values (10, 'Civil 3D', null, null, null, '2025-03-10 19:10:35.344284', 0.00, 0, 0);
-- Vendors table
insert into public.vendors (id, name, description, link_to, other_name, date_to_add, public_name, logo) values (1, 'Аскон', null, null, null, '2025-02-20 11:23:16.391870', null, null);
insert into public.vendors (id, name, description, link_to, other_name, date_to_add, public_name, logo) values (2, 'ARO', null, null, null, '2025-03-10 19:07:12.830347', null, null);
insert into public.vendors (id, name, description, link_to, other_name, date_to_add, public_name, logo) values (3, 'Яндекс', null, null, null, '2025-03-10 19:07:12.830347', null, null);
insert into public.vendors (id, name, description, link_to, other_name, date_to_add, public_name, logo) values (4, 'Tiver group', null, null, null, '2025-03-10 19:07:12.830347', null, null);
insert into public.vendors (id, name, description, link_to, other_name, date_to_add, public_name, logo) values (5, 'Наследие', null, null, null, '2025-03-10 19:07:12.830347', null, null);
insert into public.vendors (id, name, description, link_to, other_name, date_to_add, public_name, logo) values (6, 'Google', null, null, null, '2025-03-10 19:07:12.830347', null, null);
insert into public.vendors (id, name, description, link_to, other_name, date_to_add, public_name, logo) values (7, 'JetBrains', null, null, null, '2025-03-10 19:07:12.830347', null, null);
insert into public.vendors (id, name, description, link_to, other_name, date_to_add, public_name, logo) values (8, 'Microsoft', null, null, null, '2025-03-10 19:07:12.830347', null, null);
insert into public.vendors (id, name, description, link_to, other_name, date_to_add, public_name, logo) values (9, 'Autodesk', null, null, null, '2025-03-10 19:07:12.830347', null, null);
insert into public.vendors (id, name, description, link_to, other_name, date_to_add, public_name, logo) values (10, 'Apple', null, null, null, '2025-03-10 19:07:12.830347', null, null);
-- Software-vendors link table
insert into public.software_vendors_links (software_id, vendor_id) values (1, 1);
insert into public.software_vendors_links (software_id, vendor_id) values (2, 2);
insert into public.software_vendors_links (software_id, vendor_id) values (3, 3);
insert into public.software_vendors_links (software_id, vendor_id) values (4, 4);
insert into public.software_vendors_links (software_id, vendor_id) values (5, 5);
insert into public.software_vendors_links (software_id, vendor_id) values (6, 6);
insert into public.software_vendors_links (software_id, vendor_id) values (7, 7);
insert into public.software_vendors_links (software_id, vendor_id) values (8, 8);
insert into public.software_vendors_links (software_id, vendor_id) values (9, 9);
insert into public.software_vendors_links (software_id, vendor_id) values (10, 10);
-- Scope table
insert into public.guide_scope_attribute (id, scope) values (1, 'Global');

-- Guide attribute table
insert into public.guide_attribute (id, attribute_name, filterable_attribute, id_scope) values (1, 'Classification_Autodesk', true, 1);
insert into public.guide_attribute (id, attribute_name, filterable_attribute, id_scope) values (2, 'Classification_Mark', true, 1);
insert into public.guide_attribute (id, attribute_name, filterable_attribute, id_scope) values (3, 'number_programs', true, 1);
insert into public.guide_attribute (id, attribute_name, filterable_attribute, id_scope) values (4, 'process', true, 1);
insert into public.guide_attribute (id, attribute_name, filterable_attribute, id_scope) values (5, 'vendor_status', true, 1);
insert into public.guide_attribute (id, attribute_name, filterable_attribute, id_scope) values (6, 'notim', true, 1);
insert into public.guide_attribute (id, attribute_name, filterable_attribute, id_scope) values (7, 'IsHasNotim', true, 1);
insert into public.guide_attribute (id, attribute_name, filterable_attribute, id_scope) values (8, 'rr_reg_number', true, 1);
insert into public.guide_attribute (id, attribute_name, filterable_attribute, id_scope) values (9, 'rr_name', true, 1);
insert into public.guide_attribute (id, attribute_name, filterable_attribute, id_scope) values (10, 'rr_class', true, 1);
insert into public.guide_attribute (id, attribute_name, filterable_attribute, id_scope) values (11, 'analogs', true, 1);
insert into public.guide_attribute (id, attribute_name, filterable_attribute, id_scope) values (12, 'Classification_Content', true, 1);
insert into public.guide_attribute (id, attribute_name, filterable_attribute, id_scope) values (13, 'basic_software', true, 1);
insert into public.guide_attribute (id, attribute_name, filterable_attribute, id_scope) values (14, 'Classification_Stage', true, 1);
insert into public.guide_attribute (id, attribute_name, filterable_attribute, id_scope) values (15, 'Compatibility_CIM', true, 1);
insert into public.guide_attribute (id, attribute_name, filterable_attribute, id_scope) values (16, 'IsHasReestr', true, 1);
insert into public.guide_attribute (id, attribute_name, filterable_attribute, id_scope) values (17, 'Classification_Type', true, 1);
insert into public.guide_attribute (id, attribute_name, filterable_attribute, id_scope) values (18, 'Classification_Activity', true, 1);
insert into public.guide_attribute (id, attribute_name, filterable_attribute, id_scope) values (19, 'source_name', true, 1);

-- Possible values table
insert into public.attribute_possible_values (id, attribute_id, value) values (1, 2, '{ПЗ,ПЗУ,АР,КР,КЖ,КМ,ИОС,ЭОМ,ВК,ОВиК,СС,ТХ,ПОС,Смета,ТКР/ИССО,АД/ЖД}');
insert into public.attribute_possible_values (id, attribute_id, value) values (2, 1, '{Проектирование,Совместное проектирование,Координация BIM проекта,Выделение частей данных из ЦИМ,Оценка объемов,Управление лотами для закупки,Квалификация субподрядчика,Управление проектом,Управление графиком проекта,Управление стоимостью,Совместное управление строительством,Управление качеством,Управление безопасностью,Перевод объекта в эксплуатацию,Эксплуатация объекта}');
insert into public.attribute_possible_values (id, attribute_id, value) values (3, 6, '{affelated,No,Yes}');
insert into public.attribute_possible_values (id, attribute_id, value) values (4, 7, '{true,false}');
insert into public.attribute_possible_values (id, attribute_id, value) values (5, 17, '{Продукт,Плагин/Расширение}');
insert into public.attribute_possible_values (id, attribute_id, value) values (6, 16, '{true,false}');
insert into public.attribute_possible_values (id, attribute_id, value) values (7, 15, '{true,false}');
insert into public.attribute_possible_values (id, attribute_id, value) values (8, 18, '{Основное (профильное),Прикладное}');
insert into public.attribute_possible_values (id, attribute_id, value) values (9, 14, '{—,Изыскания,Проектирование,Строительство,Производство,Эксплуатация}');
insert into public.attribute_possible_values (id, attribute_id, value) values (10, 12, '{Средства управления производственными процессами (MES),Средства технологической подготовки производства (CAPP),Средства текстового и графического создания и редактирования,Средства просмотра CAD и BIM моделей,Средства администрирования и управления системами,Среда общих данных (CDE),Справочные системы,справочники,библиотеки и базы данных,Системы электронного документооборота (СЭД),Системы управления требованиями (RMS),Системы управления строительством и строительного контроля (CMS),Системы управления ресурсами предприятия (ERP),Системы управления процессами охраны труда,промышленной и пожарной безопасности (ОТиПБ),Системы управления проектами и календарно-сетевого планирования (PMS),Системы управления инженерными данными об изделии (PDM),Системы управления закупками в строительстве (SRM),Системы управление жизненным циклом продукции (PLM),Системы технического обслуживания и ремонта (ТОиР),Системы компьютерного зрения (CVS),Системы инженерного анализа и расчетов (CAE),Системы для оценки стоимости строительства (CCE),Системы диспетчерского управления и сбора данных (SCADA),Системы виртуальной и дополненной реальности (AR/VR),Системы автоматизации технологической подготовки производства (CAM),Системы CAD проектирования (CAD),Серверное и связующее программное обеспечение,Программы автоматизированного контроля качества (CAQ),Геоинформационные и навигационные системы (GIS),BI-системы (Business Intelligence),Библиотеки подпрограмм (SDK),Системы BIM проектирования (BIM)}');
-- Values attributes table
insert into public.value_attribute (id, software_id, attribute_id, value, type_data, vendor_id, _value) values (1, 1, 1, '{Проектирование}', null, null, null);
insert into public.value_attribute (id, software_id, attribute_id, value, type_data, vendor_id, _value) values (2, 2, 2, '{ПЗ}', null, null, null);
insert into public.value_attribute (id, software_id, attribute_id, value, type_data, vendor_id, _value) values (3, 3, 7, '{true}', null, null, null);
insert into public.value_attribute (id, software_id, attribute_id, value, type_data, vendor_id, _value) values (4, 4, 17, '{Продукт}', null, null, null);
insert into public.value_attribute (id, software_id, attribute_id, value, type_data, vendor_id, _value) values (5, 5, 6, '{Yes}', null, null, null);
insert into public.value_attribute (id, software_id, attribute_id, value, type_data, vendor_id, _value) values (6, 6, 18, '{Прикладное}', null, null, null);
insert into public.value_attribute (id, software_id, attribute_id, value, type_data, vendor_id, _value) values (7, 7, 18, '{Прикладное}', null, null, null);
insert into public.value_attribute (id, software_id, attribute_id, value, type_data, vendor_id, _value) values (8, 8, 14, '{Изыскания}', null, null, null);
insert into public.value_attribute (id, software_id, attribute_id, value, type_data, vendor_id, _value) values (9, 9, 17, '{Плагин/Расширение}', null, null, null);
insert into public.value_attribute (id, software_id, attribute_id, value, type_data, vendor_id, _value) values (10, 10, 17, '{Плагин/Расширение}', null, null, null);