import { Button, Divider, Form, Icon } from 'ant-design-vue';
import { WrappedFormUtils } from 'ant-design-vue/types/form/form';
import { renderFormItem, EntitiesItems } from '../utils/entitiesHelp';
import Vue, { CreateElement } from 'vue';
import { Component, Prop } from 'vue-property-decorator';
import VueI18n from 'vue-i18n';
import { EntitiesPageStore } from '@leng/public/src';
interface Entities {
    filterEntities: (props: any, h: CreateElement) => EntitiesItems
}
@Form.create({ props: ['PageStore', 'Entities'] })
@Component
export class ViewFilterBasics extends Vue {
    @Prop() form: WrappedFormUtils;
    @Prop() PageStore: EntitiesPageStore;
    @Prop() Entities: Entities;

    onSearch(body?) {
        this.PageStore.EventSubject.next({
            EventType: "onSearch",
            AjaxRequest: {
                body: {
                    Page: 1,
                    Limit: this.PageStore.PageSize,
                    ...body,
                }
            }
        });
    }
    mounted() {
        this.onSubmit();
    };
    onSubmit(e?) {
        e && e.preventDefault();
        this.form.validateFields((error, values) => {
            this.onSearch(values);
        });
    };
    onReset() {
        this.form.resetFields();
        this.onSubmit();
    };
    onToggle() {
        this.PageStore.onToggleFilterCollapse();
    };
    render(h: CreateElement) {
        const entities = this.Entities.filterEntities(this, h);
        const renderItems = renderFormItem({ entities, form: this.form }, h);
        return (
            <Form {...{ class: "page-filter-form" }} on-submit={this.onSubmit} >
                {renderItems}
                <div style="text-align: right">
                    <Button props={{ type: 'primary', icon: "search" }} html-type="submit" >提交</Button>
                    <Divider props={{ type: 'vertical' }} />
                    <Button props={{}} on-click={this.onReset} >Clear</Button>
                    <Divider props={{ type: 'vertical' }} />
                    <a on-click={this.onToggle}>
                        <span>Collapse</span>
                        <Icon props={{ type: this.PageStore.FilterCollapse ? 'up' : 'down' }} />
                    </a>
                </div>
            </Form >
        )
    }
}
// export const FormFilter = Form.create({})(FilterView);