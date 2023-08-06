<template>
    <div class="mx-auto">
        <h2 class="text-center mb-3">Import Games</h2>
        <div class="mx-auto" style="max-width:400px;">
            <div class="toast-container position-absolute sticky-top p-3 top-0 end-0" id="toastPlacement" style="margin-top: 70px;"> 
                <div ref="errortoasts" v-for="errorMessage in errorMessages" class="toast align-items-center text-white bg-danger border-0" role="alert" aria-live="assertive" aria-atomic="true">
                    <div class="d-flex">
                        <div class="toast-body">
                            <span>{{ errorMessage }}</span>
                        </div>
                        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                    </div>
                </div>           
                <div ref="successtoast" class="toast align-items-center text-white bg-success border-0" role="alert" aria-live="assertive" aria-atomic="true">
                    <div class="d-flex">
                        <div class="toast-body">
                            <span>{{ successMessage }}</span>
                        </div>
                        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                    </div>
                </div>
            </div>
            <div class="row g-2 justify-content-center mb-3">
                <div class="p-0">
                    <input type="radio" class="btn-check" name="options-outlined" id="latest-outlined" autocomplete="off" value="0" v-model="isImportAll">
                    <label class="btn btn-outline-primary me-2" for="latest-outlined">Latest</label>
                    <input type="radio" class="btn-check" name="options-outlined" id="all-outlined" autocomplete="off" value="1" v-model="isImportAll">
                    <label class="btn btn-outline-primary" for="all-outlined">All</label>
                </div>
                <button v-for="(userAccount, userAccountIndex) in importgamesvm.userAccounts" type="button" class="btn btn-outline-dark d-flex justify-content-center align-items-center" @click="onImportGamesClick(userAccount)" :disabled="selectedUserAccountID">
                    <font-awesome-icon :icon="getIconClass(userAccount.accountTypeID)" :class="getIconColorClass(userAccount.accountTypeID)" size="xl"/>
                    <div class="mx-auto">
                        <div class="align-self-start">
                            <span class="mx-auto">{{ (selectedUserAccountID == userAccount.id ? 'Importing ' : 'Import ') + userAccount.accountTypeName + ' games' }}</span>
                        </div>
                        <div v-if="userAccount.relativeImportLastRunDateString" class="align-self-end text-xs">
                            <span>{{ 'Last imported ' + userAccount.relativeImportLastRunDateString }}</span>
                        </div>
                    </div>
                    <font-awesome-icon v-if="selectedUserAccountID == userAccount.id" icon="fa-solid fa-spinner" spin size="xl"/>
                </button>
            </div>
            <div class="row g-2 justify-content-center">
                <a href="/" class="btn btn-primary mt-3" tabindex="-1" role="button">Back to my games</a>
            </div>
        </div>
    </div>
</template>
<script>
    import { Toast } from 'bootstrap';
    import axios from 'axios';
    
    export default {
        name: "ImportGames",
        props: {
            importgamesvm: Object
        },
        data() {
            return {
                selectedUserAccountID: null,   
                isImportAll: 0,           
                successToast: {},
                successMessage: '',
                errorMessages: [],
                errorToast: {}
            }
        },
        computed: {
        },
        created: function () {
        },        
        mounted: function () {
            var that = this;
            that.successToast = new Toast(that.$refs.successtoast);
            that.errorToast = new Toast(that.$refs.errortoast);

            if (that.importgamesvm.authSuccess != null) {
                if (that.importgamesvm.authSuccess) {
                    var userAccount = that.importgamesvm.userAccounts.find(i => i.accountTypeID == that.importgamesvm.authAccountTypeID);
                    that.ImportGames(userAccount);
                } else {            
                    that.errorMessages = ["Error authorizing account"];
                    if (that.errorMessages.length > 0) {
                        that.$nextTick(function() {
                            that.$refs.errortoasts?.forEach(el => {
                                new Toast(el).show();
                            });
                        }); 
                    } 
                }
            }            
        },        
        methods: {
            getIconClass: function (accountTypeID) {
                var iconClass = '';

                switch (accountTypeID) {
                    case 1:
                        iconClass = 'fa-brands fa-steam';
                        break;
                    case 2:
                        iconClass = 'fa-brands fa-xbox';
                        break;
                }

                return iconClass;
            },
            getIconColorClass: function (accountTypeID) {
                var iconClass = '';

                switch (accountTypeID) {
                    case 1:
                        iconClass = 'text-color-blue';
                        break;
                    case 2:
                        iconClass = 'text-color-green';
                        break;
                }

                return iconClass;
            },            
            onImportGamesClick(userAccount) {
                this.ImportGames(userAccount);
            },
            ImportGames(userAccount) {
                var that = this;
                that.selectedUserAccountID = userAccount.id;

                return axios.post('/Home/ImportGames', null,{ params: { userAccountID: userAccount.id, isImportAll: that.isImportAll == 1 } })
                    .then((res) => {
                        if (res.data.success) {
                            that.successMessage = "Successfully imported " + userAccount.accountTypeName + " games"
                            that.successToast.show();
                        } else {
                            that.errorMessages = ["Error importing " + userAccount.accountTypeName + " games"];
                            if (that.errorMessages.length > 0) {
                                that.$nextTick(function() {
                                    that.$refs.errortoasts?.forEach(el => {
                                        new Toast(el).show();
                                    });
                                }); 
                            }                           
                        }
                        that.selectedUserAccountID = null;
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },            
        }
    };
</script>


