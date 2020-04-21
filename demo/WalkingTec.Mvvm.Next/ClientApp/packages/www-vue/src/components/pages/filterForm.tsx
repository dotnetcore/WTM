import { Button, Divider, Form, Icon, Row, Col } from 'ant-design-vue';
import { WrappedFormUtils } from 'ant-design-vue/types/form/form';
// import { renderFormItem } from '../utils/entitiesHelp';
import Vue, { CreateElement } from 'vue';
import { Component, Prop } from 'vue-property-decorator';
import VueI18n from 'vue-i18n';
import { EntitiesPageStore } from '@leng/public/src';
import lodash from 'lodash';
import { toJS } from 'mobx';
interface Entities {
    filterEntities: (props: any, h: CreateElement) => any
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
    render(h: CreateElement, context) {
        const showLength = 5;
        // const entities = this.Entities.filterEntities(this, h);
        // const renderItems = renderFormItem({
        //     entities,
        //     form: this.form,
        //     initialValues: toJS(this.PageStore.SearchParams),
        //     ColProps: { xs: 24, sm: 24, md: 12, lg: 8 }
        // }, h);
        // const { length } = renderItems;
        // if (!this.PageStore.FilterCollapse) {
        //     renderItems.length = showLength
        // }
        // return (
        //     <Form {...{ class: "page-filter-form", form: this.form }} on-submit={this.onSubmit} >
        //         <Row props={{ gutter: 20, type: "flex" }}>
        //             {/* {renderItems} */}
        //             {this.$slots.default}
        //             <Col {...{ class: "page-filter-btns" }} >
        //                 <Button props={{ type: 'primary', icon: "search" }} html-type="submit" >提交</Button>
        //                 <Divider props={{ type: 'vertical' }} />
        //                 <Button props={{}} on-click={this.onReset} >Clear</Button>
        //                 {length > showLength && (
        //                     <span>
        //                         <Divider props={{ type: 'vertical' }} />
        //                         <a on-click={this.onToggle}>
        //                             <span>Collapse</span>
        //                             <Icon props={{ type: this.PageStore.FilterCollapse ? 'up' : 'down' }} />
        //                         </a>
        //                     </span>
        //                 )}
        //             </Col>
        //         </Row>
        //     </Form >
        // )
    }
}
// export const FormFilter = Form.create({})(FilterView);