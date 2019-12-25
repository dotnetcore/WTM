import { Form, Icon, Button, Divider, Col } from 'ant-design-vue';
import { WrappedFormUtils } from 'ant-design-vue/types/form/form';
import Vue, { CreateElement } from 'vue';
import { Component, Prop } from 'vue-property-decorator';
import PageStore from "../store";
import Models from './models';
import lodash from 'lodash';
@Form.create({ props: ['PageStore'] })
@Component
export class ViewFilter extends Vue {
    @Prop() form: WrappedFormUtils;
    @Prop() PageStore: PageStore;
    onSearch(body?) {
        this.PageStore.EventSubject.next({
            EventType: "onSearch",
            AjaxRequest: {
                body: {
                    Page: 1,
                    Limit: 20,
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
        const models = Models.filterModels({}, h);
        const renderItems = Models.renderModels({ models, form: this.form }, h);
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