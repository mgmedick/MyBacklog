<template>
    <div class="mx-auto">
        <h2 class="text-center mb-3">Create Account</h2>
        <div class="mx-auto" style="max-width:400px;">
            <form v-if="islinkvalid" @submit.prevent="submitForm" autocomplete="off">
                <div id="toastPlacement" ref="toastcontainer" class="toast-container position-fixed top-0 end-0" style="margin-top:70px;"> 
                    <div ref="errortoast" class="toast align-items-center text-white bg-danger border-0" role="alert" aria-live="assertive" aria-atomic="true">
                        <div class="d-flex">
                            <div class="toast-body">
                                <span class="msg-text"></span>
                            </div>
                            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                        </div>
                    </div>              
                    <div ref="successtoast" class="toast align-items-center text-white bg-success border-0" role="alert" aria-live="assertive" aria-atomic="true">
                        <div class="d-flex">
                            <div class="toast-body">
                                <span class="msg-text"></span>
                            </div>
                            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                        </div>
                    </div>                                    
                </div>
                <div class="mb-2">
                    <label for="txtEmail" class="form-label">Email</label>
                    <input id="txtEmail" type="text" class="form-control" v-model.lazy="form.Email" disabled>
                </div>              
                <div class="mb-2">
                    <label for="txtUserName" class="form-label">Username</label>
                    <input id="txtUserName" type="text" class="form-control" autocomplete="off" v-model.lazy="form.Username" @blur="v$.form.Username.$touch" aria-describedby="spnUserNameErrors">
                    <div>
                        <span id="spnUserNameErrors" class="form-text text-danger" v-for="error of v$.form.Username.$errors">{{ error.$message }}</span>
                    </div>
                </div>
                <div class="mb-2">
                    <label for="txtPassword" class="form-label">Password</label>
                    <input id="txtPassword" type="password" class="form-control" autocomplete="off" v-model.lazy="form.Password" @blur="v$.form.Password.$touch" aria-describedby="spnPasswordErrors">
                    <div>
                        <span id="spnPasswordErrors" class="form-text text-danger" v-for="error of v$.form.Password.$errors">{{ error.$message }}</span>
                    </div>
                </div>
                <div class="mb-3">
                    <label for="txtConfirmPassword" class="form-label">Confirm Password</label>
                    <input id="txtConfirmPassword" type="password" class="form-control" autocomplete="off" v-model.lazy="form.ConfirmPassword" @blur="v$.form.ConfirmPassword.$touch" aria-describedby="spnConfirmPasswordErrors">
                    <div>
                        <span id="spnConfirmPasswordErrors" class="form-text text-danger" v-for="error of v$.form.ConfirmPassword.$errors">{{ error.$message }}</span>
                    </div>
                </div>
                <div class="row g-2 justify-content-center mb-3 mx-auto">
                    <button type="submit" class="btn btn-primary d-flex justify-content-center align-items-center">Submit</button>
                </div> 
                <div ref="loadingmodal" class="modal" tabindex="-1">
                    <div class="modal-dialog modal-dialog-centered justify-content-center" style="color: #fff;">
                        <font-awesome-icon icon="fa-solid fa-spinner" spin size="2xl" />
                    </div>
                </div>                                    
            </form>
            <div v-else class="text-center">
                <div class="m-3">
                    <font-awesome-icon icon="fa-solid fa-hourglass-end" size="2xl" />
                </div>
                <div>
                    <span>Activation link has expired, please try again.</span>
                    <div>
                        <a href="/Signup">Sign Up</a>
                    </div>
                </div>
            </div>   
        </div> 
    </div>
</template>
<script>
    import { getFormData, setCookie } from '../../js/common.js';
    import axios from 'axios';
    import useVuelidate from '@vuelidate/core';
    import { required, helpers, sameAs } from '@vuelidate/validators';
    import { Toast } from 'bootstrap';
    const { withAsync } = helpers;

    const usernameFormat = helpers.regex(/^[._()-\/#&$@+\w\s]{3,30}$/)
    const passwordFormat = helpers.regex(/^(?=.*[A-Za-z])(?=.*\d)[._()-\/#&$@+\w\s]{8,30}$/)

    const asyncUsernameNotExists = async (value) => {  
        if(value === '') { return true; };            

        return await axios.get('/Home/UsernameNotExists', { params: { username: value } })
            .then(res => {
                return Promise.resolve(res.data);
            })
            .catch(err => { console.error(err); return Promise.reject(err); });
    }

    export default {
        name: "Activate",
        props: {
            islinkvalid: Boolean,
            email: String
        },
        setup() {
            return { v$: useVuelidate() }
        },
        data() {
            return {
                form: {
                    Email: this.email,
                    Username: '',
                    Password: '',
                    ConfirmPassword: ''
                },
                loadingModal: {}
            }
        },
        computed: {
        },
        created: function () {
        },
        mounted: function () {
            this.loadingModal = new Modal(this.$refs.loadingmodal);
        },        
        methods: {
            async submitForm() {
                const isValid = await this.v$.$validate()
                if (!isValid) return

                var that = this;
                var formData = getFormData(this.form);
                this.loadingModal.show();

                axios.post('/Home/Activate', formData)
                    .then((res) => {
                        if (res.data.success) {
                            location.href = '/Welcome';
                        } else {
                            res.data.errorMessages.forEach(errorMsg => {
                                that.onError(errorMsg);                           
                            });                              
                        }
                                                                        
                        that.loadingModal.hide();
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },
            onSuccess(successMsg) {
                var that = this;
                var el = that.$refs.successtoast.cloneNode(true);
                el.querySelector('.msg-text').innerHTML = successMsg;
                that.$refs.toastcontainer.appendChild(el);
                new Toast(el).show();    
            },
            onError(errorMsg) {
                var that = this;
                var el = that.$refs.errortoast.cloneNode(true);
                el.querySelector('.msg-text').innerHTML = errorMsg;
                that.$refs.toastcontainer.appendChild(el);
                new Toast(el).show();  
            }                                   
        },
        validations() {
            return {
                form: {
                    Username: {
                        required: helpers.withMessage('Username is required', required),
                        usernameFormat: helpers.withMessage('Must be between 3 - 30 characters', usernameFormat),
                        usernameNotExists: helpers.withMessage('Username already exists', withAsync(asyncUsernameNotExists))
                    },
                    Password: {
                        required: helpers.withMessage('Password is required', required),
                        passwordFormat: helpers.withMessage('Must be between 8 - 30 characters with at least 1 number and letter', passwordFormat)
                    },
                    ConfirmPassword: {
                        sameAsPassword: helpers.withMessage('Must match Password', sameAs(this.form.Password))
                    }
                }
            }
        }
    };
</script>


