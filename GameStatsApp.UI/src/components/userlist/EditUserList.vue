<template>
    <form @submit.prevent="submitForm" autocomplete="off">
        <div class="form-check">
            <input class="form-check-input" type="checkbox" v-model="form.active" />
            <label class="form-check-label">
                Active
            </label>
        </div>
        <div class="mb-3">
            <input type="text" class="form-control" autocomplete="off" v-model.lazy="form.name" @blur="v$.form.name.$touch"  aria-describedby="spnNameErrors" />
            <div>
                <div id="spnNameErrors" class="form-text text-danger" v-for="error of v$.form.name.$errors">{{ error.$message }}</div>
            </div>
        </div>      
    </form>
</template>
<script>
    import { getFormData, successToast, errorToast } from '../../js/common.js';
    import axios from 'axios';
    import useVuelidate from '@vuelidate/core';
    import { required, helpers } from '@vuelidate/validators';  
    
    const { withAsync } = helpers;
    const userListNameFormat = helpers.regex(/^[._()-\/#&$@+\w\s]{3,30}$/)

    export default {
        name: "EditUserList",
        emits: ["saved"],
        props: {
            userlist: Object
        },
        setup() {
            return { v$: useVuelidate() }
        },
        data() {
            return {
                form: {
                    id: 0,
                    name: '',
                    active: true,
                    sortOrder: 0
                }
            }
        },    
        watch: {
            userlist: {
                handler(val, oldVal) {
                    this.form.id = val.id;
                    this.form.name = val.name;
                    this.form.active = val.active;
                    this.form.sortOrder = val.sortOrder;
                    this.v$.$reset();
                },
                deep: true
            }             
        },                    
        mounted: function () {
        },
        methods: {
            async submitForm() {
                const isValid = await this.v$.$validate()
                if (!isValid) return

                var that = this;
                var formData = getFormData(this.form);
                var config = { headers: { 'RequestVerificationToken': that.getCsrfToken() } };

                return await axios.post('/UserList/ManageUserLists', formData, config).then((res) => {
                    that.$emit('saved', res.data.success);
                    if (res.data.success) {
                        successToast("Saved <strong>" + that.form.name + "</strong> list");                           
                    } else {
                        res.data.errorMessages.forEach(errorMsg => {
                            errorToast(errorMsg);                           
                        });                                
                    }
                })
                .catch(err => { console.error(err); return Promise.reject(err); });        
            },
            async userListNameNotExists(value) {  
                if (value === '' || value == this.userlist.name) {
                    return true; 
                };            

                return await axios.get('/UserList/UserListNameNotExists', { params: { userListID: this.form.id, userListName: value } })
                    .then(res => {
                        return Promise.resolve(res.data);
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            }              
        },
        validations() {
            return {
                form: {
                    name: {
                        required: helpers.withMessage('List name is required', required),
                        userListNameFormat: helpers.withMessage('Must be between 3 - 30 characters', userListNameFormat),
                        userListNameNotExists: helpers.withMessage('List name already exists', withAsync(this.userListNameNotExists))
                    },
                }
            }
        }
    };
</script>


