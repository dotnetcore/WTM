


import { $i18n, WTM_EntitiesField, WTM_ValueType, FieldRequest, $locales } from '@/client';
import router from '@/router';
import { ColGroupDef, GridOptions } from 'ag-grid-community';
import { ColDef } from 'ag-grid-community';
import lodash from 'lodash';
import language from '@/client/locales/languagesys';

/**
 * 页面实体
 */

class StudentEntity {
    
    readonly IDAdd: WTM_EntitiesField = {
        name: ['Entity', 'ID'],
        label: 'ID',
        rules:[{ required: true }],
        valueType: WTM_ValueType.text,
    }

    readonly PasswordAdd: WTM_EntitiesField = {
        name: ['Entity', 'Password'],
        label: '_Model_Student_Password',
        rules:[{ required: true }],
        valueType: WTM_ValueType.text,
    }
    readonly EmailAdd: WTM_EntitiesField = {
        name: ['Entity', 'Email'],
        label: '_Model_Student_Email',
        valueType: WTM_ValueType.text,
    }
    readonly NameAdd: WTM_EntitiesField = {
        name: ['Entity', 'Name'],
        label: '_Model_Student_Name',
        rules:[{ required: true }],
        valueType: WTM_ValueType.text,
    }
    readonly SexAdd: WTM_EntitiesField = {
        name: ['Entity', 'Sex'],
        label: '_Model_Student_Sex',
        request: async (formState) => {
            return [
                
                { label: '男', value: 'Male' },
                { label: '女', value: 'Female' },
            ]
        },
        valueType: WTM_ValueType.select,
    }
    readonly CellPhoneAdd: WTM_EntitiesField = {
        name: ['Entity', 'CellPhone'],
        label: '_Model_Student_CellPhone',
        valueType: WTM_ValueType.text,
    }
    readonly AddressAdd: WTM_EntitiesField = {
        name: ['Entity', 'Address'],
        label: '_Model_Student_Address',
        valueType: WTM_ValueType.text,
    }
    readonly ZipCodeAdd: WTM_EntitiesField = {
        name: ['Entity', 'ZipCode'],
        label: '_Model_Student_ZipCode',
        valueType: WTM_ValueType.text,
    }
    readonly PhotoIdAdd: WTM_EntitiesField = {
        name: ['Entity', 'PhotoId'],
        label: '_Model_Student_Photo',
        valueType: WTM_ValueType.image,
    }
    readonly FileIdAdd: WTM_EntitiesField = {
        name: ['Entity', 'FileId'],
        label: '_Model_Student_File',
        valueType: WTM_ValueType.upload,
    }
    readonly IsValidAdd: WTM_EntitiesField = {
        name: ['Entity', 'IsValid'],
        label: '_Model_Student_IsValid',
        request: async (formState) => {
            return [
                { label: $i18n.t($locales.tips_bool_true), value: true },
                { label: $i18n.t($locales.tips_bool_false), value: false }
            ]
        },
        valueType: WTM_ValueType.select,
    }
    readonly EnRollDateAdd: WTM_EntitiesField = {
        name: ['Entity', 'EnRollDate'],
        label: '_Model_Student_EnRollDate',
        rules:[{ required: true }],
        valueType: WTM_ValueType.date,
    }
    readonly SelectedMajorStudent_MT_WtmsIDsAdd: WTM_EntitiesField = {
        name: 'SelectedMajorStudent_MT_WtmsIDs',
        request: async () => FieldRequest('/api/Student/GetMajors'),
        fieldProps: { mode: 'tags' },
        label: '_Model_Student_StudentMajor',
        valueType: WTM_ValueType.select,
    }
    readonly PasswordEdit: WTM_EntitiesField = {
        name: ['Entity', 'Password'],
        label: '_Model_Student_Password',
        rules:[{ required: true }],
        valueType: WTM_ValueType.text,
    }
    readonly EmailEdit: WTM_EntitiesField = {
        name: ['Entity', 'Email'],
        label: '_Model_Student_Email',
        valueType: WTM_ValueType.text,
    }
    readonly NameEdit: WTM_EntitiesField = {
        name: ['Entity', 'Name'],
        label: '_Model_Student_Name',
        rules:[{ required: true }],
        valueType: WTM_ValueType.text,
    }
    readonly SexEdit: WTM_EntitiesField = {
        name: ['Entity', 'Sex'],
        label: '_Model_Student_Sex',
        request: async (formState) => {
            return [
                
                { label: '男', value: 'Male' },
                { label: '女', value: 'Female' },
            ]
        },
        valueType: WTM_ValueType.select,
    }
    readonly CellPhoneEdit: WTM_EntitiesField = {
        name: ['Entity', 'CellPhone'],
        label: '_Model_Student_CellPhone',
        valueType: WTM_ValueType.text,
    }
    readonly AddressEdit: WTM_EntitiesField = {
        name: ['Entity', 'Address'],
        label: '_Model_Student_Address',
        valueType: WTM_ValueType.text,
    }
    readonly ZipCodeEdit: WTM_EntitiesField = {
        name: ['Entity', 'ZipCode'],
        label: '_Model_Student_ZipCode',
        valueType: WTM_ValueType.text,
    }
    readonly PhotoIdEdit: WTM_EntitiesField = {
        name: ['Entity', 'PhotoId'],
        label: '_Model_Student_Photo',
        valueType: WTM_ValueType.image,
    }
    readonly FileIdEdit: WTM_EntitiesField = {
        name: ['Entity', 'FileId'],
        label: '_Model_Student_File',
        valueType: WTM_ValueType.upload,
    }
    readonly IsValidEdit: WTM_EntitiesField = {
        name: ['Entity', 'IsValid'],
        label: '_Model_Student_IsValid',
        request: async (formState) => {
            return [
                { label: $i18n.t($locales.tips_bool_true), value: true },
                { label: $i18n.t($locales.tips_bool_false), value: false }
            ]
        },
        valueType: WTM_ValueType.select,
    }
    readonly EnRollDateEdit: WTM_EntitiesField = {
        name: ['Entity', 'EnRollDate'],
        label: '_Model_Student_EnRollDate',
        rules:[{ required: true }],
        valueType: WTM_ValueType.date,
    }
    readonly SelectedMajorStudent_MT_WtmsIDsEdit: WTM_EntitiesField = {
        name: 'SelectedMajorStudent_MT_WtmsIDs',
        request: async () => FieldRequest('/api/Student/GetMajors'),
        fieldProps: { mode: 'tags' },
        label: '_Model_Student_StudentMajor',
        valueType: WTM_ValueType.select,
    }
    readonly Password_Filter : WTM_EntitiesField = {
        name: 'Password',
        label: '_Model_Student_Password',
        valueType: WTM_ValueType.text,
    }
    readonly Email_Filter : WTM_EntitiesField = {
        name: 'Email',
        label: '_Model_Student_Email',
        valueType: WTM_ValueType.text,
    }
    readonly Name_Filter : WTM_EntitiesField = {
        name: 'Name',
        label: '_Model_Student_Name',
        valueType: WTM_ValueType.text,
    }
    readonly Sex_Filter : WTM_EntitiesField = {
        name: 'Sex',
        label: '_Model_Student_Sex',
        valueType: WTM_ValueType.select,
        request: async (formState) => {
            return [
                
                { label: '男', value: 'Male' },
                { label: '女', value: 'Female' },
            ]
        },
    }
    readonly CellPhone_Filter : WTM_EntitiesField = {
        name: 'CellPhone',
        label: '_Model_Student_CellPhone',
        valueType: WTM_ValueType.text,
    }
    readonly Address_Filter : WTM_EntitiesField = {
        name: 'Address',
        label: '_Model_Student_Address',
        valueType: WTM_ValueType.text,
    }
    readonly ZipCode_Filter : WTM_EntitiesField = {
        name: 'ZipCode',
        label: '_Model_Student_ZipCode',
        valueType: WTM_ValueType.text,
    }
    readonly EnRollDate_Filter : WTM_EntitiesField = {
        name: 'EnRollDate',
        label: '_Model_Student_EnRollDate',
        valueType: WTM_ValueType.dateRange,
    }
    readonly SelectedMajorStudent_MT_WtmsIDs_Filter : WTM_EntitiesField = {
        name: 'SelectedMajorStudent_MT_WtmsIDs',
        label: '_Model_Student_StudentMajor',
        fieldProps: { mode: 'tags' },
        request: async () => FieldRequest('/api/Student/GetMajors'),
        valueType: WTM_ValueType.select,
    }
    readonly CreateTime_Filter : WTM_EntitiesField = {
        name: 'CreateTime',
        label: '_Model_Student_CreateTime',
        valueType: WTM_ValueType.dateRange,
    }
    readonly UpdateTime_Filter : WTM_EntitiesField = {
        name: 'UpdateTime',
        label: '_Model_Student_UpdateTime',
        valueType: WTM_ValueType.dateRange,
    }
    readonly CreateBy_Filter : WTM_EntitiesField = {
        name: 'CreateBy',
        label: '_Model_Student_CreateBy',
        valueType: WTM_ValueType.text,
    }
    readonly UpdateBy_Filter : WTM_EntitiesField = {
        name: 'UpdateBy',
        label: '_Model_Student_UpdateBy',
        valueType: WTM_ValueType.text,
    }
    readonly PasswordDetail: WTM_EntitiesField = {
        name: ['Entity', 'Password'],
        label: '_Model_Student_Password',
        rules:[{ required: true }],
        valueType: WTM_ValueType.text,
    }
    readonly EmailDetail: WTM_EntitiesField = {
        name: ['Entity', 'Email'],
        label: '_Model_Student_Email',
        valueType: WTM_ValueType.text,
    }
    readonly NameDetail: WTM_EntitiesField = {
        name: ['Entity', 'Name'],
        label: '_Model_Student_Name',
        rules:[{ required: true }],
        valueType: WTM_ValueType.text,
    }
    readonly SexDetail: WTM_EntitiesField = {
        name: ['Entity', 'Sex'],
        label: '_Model_Student_Sex',
        request: async (formState) => {
            return [
                
                { label: '男', value: 'Male' },
                { label: '女', value: 'Female' },
            ]
        },
        valueType: WTM_ValueType.select,
    }
    readonly CellPhoneDetail: WTM_EntitiesField = {
        name: ['Entity', 'CellPhone'],
        label: '_Model_Student_CellPhone',
        valueType: WTM_ValueType.text,
    }
    readonly AddressDetail: WTM_EntitiesField = {
        name: ['Entity', 'Address'],
        label: '_Model_Student_Address',
        valueType: WTM_ValueType.text,
    }
    readonly ZipCodeDetail: WTM_EntitiesField = {
        name: ['Entity', 'ZipCode'],
        label: '_Model_Student_ZipCode',
        valueType: WTM_ValueType.text,
    }
    readonly PhotoIdDetail: WTM_EntitiesField = {
        name: ['Entity', 'PhotoId'],
        label: '_Model_Student_Photo',
        valueType: WTM_ValueType.image,
    }
    readonly FileIdDetail: WTM_EntitiesField = {
        name: ['Entity', 'FileId'],
        label: '_Model_Student_File',
        valueType: WTM_ValueType.upload,
    }
    readonly IsValidDetail: WTM_EntitiesField = {
        name: ['Entity', 'IsValid'],
        label: '_Model_Student_IsValid',
        request: async (formState) => {
            return [
                { label: $i18n.t($locales.tips_bool_true), value: true },
                { label: $i18n.t($locales.tips_bool_false), value: false }
            ]
        },
        valueType: WTM_ValueType.text,
    }
    readonly EnRollDateDetail: WTM_EntitiesField = {
        name: ['Entity', 'EnRollDate'],
        label: '_Model_Student_EnRollDate',
        rules:[{ required: true }],
        valueType: WTM_ValueType.text,
    }
    readonly SelectedMajorStudent_MT_WtmsIDsDetail: WTM_EntitiesField = {
        name: 'SelectedMajorStudent_MT_WtmsIDs',
        request: async () => FieldRequest('/api/Student/GetMajors'),
        fieldProps: { mode: 'tags' },
        label: '_Model_Student_StudentMajor',
        valueType: WTM_ValueType.checkbox,
    }
    readonly CreateTimeDetail: WTM_EntitiesField = {
        name: ['Entity', 'CreateTime'],
        label: '_Model_Student_CreateTime',
        valueType: WTM_ValueType.text,
    }
    readonly UpdateTimeDetail: WTM_EntitiesField = {
        name: ['Entity', 'UpdateTime'],
        label: '_Model_Student_UpdateTime',
        valueType: WTM_ValueType.text,
    }
    readonly CreateByDetail: WTM_EntitiesField = {
        name: ['Entity', 'CreateBy'],
        label: '_Model_Student_CreateBy',
        valueType: WTM_ValueType.text,
    }
    readonly UpdateByDetail: WTM_EntitiesField = {
        name: ['Entity', 'UpdateBy'],
        label: '_Model_Student_UpdateBy',
        valueType: WTM_ValueType.text,
    }
    readonly LinkedVM_Password: WTM_EntitiesField = {
        name: ['LinkedVM', 'Password'],
        label: '_Model_Student_Password',
        rules:[{ required: true }],
        valueType: WTM_ValueType.text,
    }
    readonly LinkedVM_Email: WTM_EntitiesField = {
        name: ['LinkedVM', 'Email'],
        label: '_Model_Student_Email',
        valueType: WTM_ValueType.text,
    }
    readonly LinkedVM_Name: WTM_EntitiesField = {
        name: ['LinkedVM', 'Name'],
        label: '_Model_Student_Name',
        rules:[{ required: true }],
        valueType: WTM_ValueType.text,
    }
    readonly LinkedVM_Sex: WTM_EntitiesField = {
        name: ['LinkedVM', 'Sex'],
        label: '_Model_Student_Sex',
        request: async (formState) => {
            return [
                
                { label: '男', value: 'Male' },
                { label: '女', value: 'Female' },
            ]
        },
        valueType: WTM_ValueType.select,
    }
    readonly LinkedVM_CellPhone: WTM_EntitiesField = {
        name: ['LinkedVM', 'CellPhone'],
        label: '_Model_Student_CellPhone',
        valueType: WTM_ValueType.text,
    }
    readonly LinkedVM_Address: WTM_EntitiesField = {
        name: ['LinkedVM', 'Address'],
        label: '_Model_Student_Address',
        valueType: WTM_ValueType.text,
    }
    readonly LinkedVM_ZipCode: WTM_EntitiesField = {
        name: ['LinkedVM', 'ZipCode'],
        label: '_Model_Student_ZipCode',
        valueType: WTM_ValueType.text,
    }
    readonly LinkedVM_IsValid: WTM_EntitiesField = {
        name: ['LinkedVM', 'IsValid'],
        label: '_Model_Student_IsValid',
        valueType: WTM_ValueType.select,
        request: async (formState) => {
            return [
                { label: $i18n.t($locales.tips_bool_true), value: true },
                { label: $i18n.t($locales.tips_bool_false), value: false }
            ]
        },
    }
    readonly LinkedVM_EnRollDate: WTM_EntitiesField = {
        name: ['LinkedVM', 'EnRollDate'],
        label: '_Model_Student_EnRollDate',
        rules:[{ required: true }],
        valueType: WTM_ValueType.date,
    }
    readonly LinkedVM_SelectedMajorStudent_MT_WtmsIDs: WTM_EntitiesField = {
        name: ['LinkedVM','SelectedMajorStudent_MT_WtmsIDs'],
        request: async () => FieldRequest('/api/Student/GetMajors'),
        fieldProps: { mode: 'tags' },
        label: '_Model_Student_StudentMajor',
        valueType: WTM_ValueType.select,
    }
}

export const ExStudentEntity = new StudentEntity()

