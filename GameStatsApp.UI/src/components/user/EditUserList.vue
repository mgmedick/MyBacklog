<template>
    <form ref="form" @submit.prevent="submitForm" autocomplete="off">
        <div class="mb-3">
            <input id="txtUserListName" type="text" class="form-control" autocomplete="off" v-model.lazy="form.UserListName" @blur="v$.form.UserListName.$touch"  aria-describedby="spnUserListNameErrors">
            <div>
                <span id="spnUserListNameErrors" class="form-text text-danger" v-for="error of v$.form.UserListName.$errors">{{ error.$message }}</span>
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

    const asyncUserListNameNotExists = async (value) => {  
        if(value === '' || value === this.origUserListName) { return true; };            

        return await axios.get('/User/UserListNameNotExists', { params: { userListName: value } })
            .then(res => {
                return Promise.resolve(res.data);
            })
            .catch(err => { console.error(err); return Promise.reject(err); });
    }

    export default {
        name: "EditUserList",
        emits: ["complete"],
        props: {
            userlist: Object
        },
        setup() {
            return { v$: useVuelidate() }
        },
        data() {
            return {
                form: {
                    UserListID: this.userlist.id,
                    UserListName: this.userlist.name
                },
                origUserListName: this.userlist.name
            }
        },  
        watch: {
            userlist: function (val, oldVal) {
                if (val.id != oldVal.id) {
                    this.form.UserListID = val.id;
                    this.form.UserListName = val.name;
                }
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
                this.loading = true;

                return axios.post('/User/SaveUserList', formData)
                    .then((res) => {
                        this.$emit('complete', res);
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },
            async userListNameNotExists(value) {  
                if (value === '' || value == (this.origUserListName ?? this.userlist.name)) { return true; };            

                return await axios.get('/User/UserListNameNotExists', { params: { userListName: value } })
                    .then(res => {
                        return Promise.resolve(res.data);
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            }                  
        },
        validations() {
            return {
                form: {
                    UserListName: {
                        required: helpers.withMessage('List name is required', required),
                        userListNameFormat: helpers.withMessage('Must be between 3 - 30 characters', userListNameFormat),
                        userListNameNotExists: helpers.withMessage('List name already exists', withAsync(this.userListNameNotExists))
                    },
                }
            }
        }
    };
</script>


