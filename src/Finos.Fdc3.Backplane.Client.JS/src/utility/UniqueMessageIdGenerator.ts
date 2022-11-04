/**
 * SPDX-License-Identifier: Apache-2.0
 * Copyright 2021 FINOS FDC3 contributors - see NOTICE file
 */

export const getUniqueMessageId = (): string => {
	return Date.now() + '_' + Math.random();
};
